using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.AI.Paths;
using Assets.Scripts.AI.Platform;
using Assets.Scripts.MobScripts.MobManagement;
using Assets.Scripts.NetworkManagement;
using Assets.Scripts.TrackingDictionaries;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    public class Mob : MonoBehaviour
    {
        public string PlatformName = "Platform";
        public int SyncUpdateInMilliseconds = 3000;

        private bool _trackerInserted; // Is the mob placed into the tracker dictionary?

        private GameObject    _pathObject; // The AStarPath wrapper game object that the mob is following in the beginning
        private AStarPath     _path;       // The Path that is on top of the game object that the mob is currently following

        private MapPlatform   _mapPlatform;    // The map platform of the current scene.

        private MobAttributes _mobAttributesReference;
        public  MobAttributes MobAttributesMax;        // the mob's default attributes.
        public  MobAttributes MobAttributesCurrent;    // The mob's current attributes.

        private int           _currPathIndex;
        private List<Vector3> _currPathList = new List<Vector3>();

        public int    PlayerNumber { private set; get; }
        public int    MobNumber    { private set; get; }
        public string MobName      { private set; get; }
        public int    TeamGroup    { private set; get; }
        public string PathName     { private set; get; }

        [HideInInspector] public uint MobHash = 0;
        [HideInInspector] public bool Dead;
        [HideInInspector] public Vector3 Destination;

        private Vector3 _prevLocation;
        private TileData _currentTile;

        private readonly Stopwatch _syncStopwatch = new Stopwatch();

        private MobModifierRpc _mobModifierRpc;

        private bool _initialized;

        public void Initialize( int playerNumber, int mobNumber, string mobName, int teamGroup, string pathName )
        {
            if (_initialized)
            {
                UnityEngine.Debug.Log("" + playerNumber + "_" + mobName + " already initialized.");
                return;
            }
            if ( !MobDictionary.Instance.MobFullNameToAttributes.TryGetValue( "" + mobNumber + "_" + mobName,
                out _mobAttributesReference) )
            {
                UnityEngine.Debug.Log(" Can't find " + playerNumber + "_" + mobName + " in the Dictionary.");
                return;
            }
            PlayerNumber = playerNumber;
            MobNumber    = mobNumber;
            MobName      = mobName;
            TeamGroup    = teamGroup;
            PathName     = pathName;
            MobAttributesMax     = new MobAttributes( _mobAttributesReference );
            MobAttributesCurrent = new MobAttributes( _mobAttributesReference );
            Destination = transform.position;
            _prevLocation = transform.position;
            _mobModifierRpc = MobModifierRpcServer.Instance;
            _mapPlatform = MapPlatform.Instance;
            _initialized = true;
        }

        protected void FixedUpdate ()
        {
            if (!_initialized)
            {
                UnityEngine.Debug.Log( "Mob Not Initialized." );
                return;
            }
            _handleTileLocations();
            _handleTrackerDictionary();
            _handleMovement();
            _handleAutoSync();
        }

        protected void OnDestroy()
        {
            MobTrackerDictionary.Instance.DeleteEntry( MobHash );
        }

        private void _handleTileLocations()
        {
            if ( MobAttributesCurrent == null || MobAttributesMax == null )
            {
                MobDictionary.Instance.MobNameToAttributes.TryGetValue( MobName, out MobAttributesCurrent );
                MobDictionary.Instance.MobNameToAttributes.TryGetValue( MobName, out MobAttributesMax     );
                return;
            }

            if ( Dead ) return;

            if (_mapPlatform == null)
            {
                _mapPlatform = MapPlatform.Instance;
                return;
            }

            var nextTile = _mapPlatform.GetTile( transform.position );
            if (nextTile == null) return;

            _currentTile = nextTile;
            foreach (var t in _currentTile.TurretZonesList)
            {
                t.AlertOfMobPresence(transform.gameObject);
            }
        }

        private void _handleTrackerDictionary()
        {
            if ( MobHash == 0 || _trackerInserted ) return;
            _trackerInserted = true;
            MobTrackerDictionary.Instance.InsertEntry( MobHash, gameObject );
        }

        private void _handleMovement()
        {
            // Do not move if we do not have a path to move to!
            if ( PathName == null ) { return; } 

            // Do not move the mob if its dead. corpses dont move!
            if ( Dead ) { return; }

            // Do not move if not tracked.
            if ( MobHash == 0 ) { return; }

            if ( NetworkingManager.Instance.IsServer )
            {
                #region Server-sided movement handler 

                #region Continuously find the platform and path objects.

                if ( _mapPlatform == null)
                {
                    _mapPlatform = MapPlatform.Instance;
                    return;
                }

                if (_pathObject == null)
                {
                    AStarPath.AStarPathDictionary.TryGetValue(PathName, out _pathObject);
                    if (!_pathObject) return;
                    _path = _pathObject.GetComponent<AStarPath>();
                    _currPathIndex = _path.CompactPath.Count - 1;
                    _currPathList = new List<Vector3>(_path.CompactPath);
                    Destination = _currPathList[_currPathIndex];
                    return;
                }

                #endregion

                #region Check for path object and a checkpoint list.

                if ( _path == null ) return;

                if ( _currPathList.Count == 0 )
                {
                    _currPathList    = new List<Vector3>( _path.CompactPath );
                    _currPathIndex   = _currPathList.Count - 1;
                    Destination = _currPathList[ _currPathIndex ];
                    _mobModifierRpc.UpdateMoveStateSendRpc( MobHash, transform.position, Destination );
                    return;
                }

                #endregion

                var currPosition = transform.position;
                var endPos  = MobAttributesCurrent.Flying ? _currPathList[0] : Destination;
                    endPos.y = currPosition.y;
                var dir = Vector3.Normalize(endPos - currPosition);
                var remainingDistance = Vector3.Distance(currPosition, endPos);
                var travelDistance = MobAttributesCurrent.MoveSpeed * Time.fixedDeltaTime;
                var nextPosition = currPosition + (dir * travelDistance);

                var distanceAvaliable = remainingDistance > travelDistance;
                var currPosIsBlocked  = _mapPlatform.LocationIsBlocked( currPosition );
                var nextPosIsBlocked  = _mapPlatform.LocationIsBlocked( nextPosition );

                #region Blocked path handler.

                if ( !currPosIsBlocked && nextPosIsBlocked && !MobAttributesCurrent.Flying )
                {
                    List<Vector3> placeHolder;
                    _mapPlatform.FindAStarPath( currPosition, _path.EndPoint.transform.position, out _currPathList, out placeHolder );
                    _currPathIndex = _currPathList.Count - 1;
                    Destination = _currPathList[ _currPathIndex ];
                    _mobModifierRpc.UpdateMoveStateSendRpc( MobHash, transform.position, Destination );
                    return;
                }

                #endregion

                #region Movement and path change handler.

                if ( distanceAvaliable )
                {
                    transform.position = nextPosition;
                }
                else
                {
                    transform.position = endPos;

                    if ( --_currPathIndex >= 0 )
                    {
                        Destination = _currPathList[ _currPathIndex ];
                        _mobModifierRpc.UpdateMoveStateSendRpc( MobHash, transform.position, Destination );
                    }
                    else
                    {
                        var nextPath = _path.NextPath.GetComponent<AStarPath>();
                        _path = (nextPath == null || nextPath == _path) ? null : nextPath;
                        if (_path != null)
                        {
                            _currPathIndex   = _path.CompactPath.Count - 1;
                            _currPathList    = new List<Vector3>( _path.CompactPath );
                            Destination = _currPathList[ _currPathIndex ];
                            _mobModifierRpc.UpdateMoveStateSendRpc( MobHash, transform.position, Destination );
                        }
                    }
                }

                if ( _prevLocation != transform.position )
                {
                    transform.LookAt( 2 * transform.position - _prevLocation );
                }
                _prevLocation = transform.position;

                #endregion

                #endregion
            }
            else
            {
                #region Client-sided movement handler
                
                if ( transform.position != Destination )
                {
                    transform.LookAt( Destination );
                }
                
                var currPos = transform.position;
                var endPos = Destination;
                    endPos.y = currPos.y;
                var dir = Vector3.Normalize( endPos - currPos );
                var remainingDistance = Vector3.Distance( currPos, endPos );
                var travelDistance = MobAttributesCurrent.MoveSpeed * Time.fixedDeltaTime;
                var nextPosition = currPos + (dir * travelDistance);

                var distanceAvaliable = remainingDistance > travelDistance;

                if ( distanceAvaliable )
                {
                    transform.position = nextPosition;
                }

                #endregion
            }
        }

        private void _handleAutoSync()
        {
            if ( NetworkingManager.Instance.IsClient ) return;

            if (!_syncStopwatch.IsRunning)
            {
                _syncStopwatch.Reset();
                _syncStopwatch.Start();
                return;
            }

            if ( _syncStopwatch.ElapsedMilliseconds >= SyncUpdateInMilliseconds )
            {
                _syncStopwatch.Stop();
                _mobModifierRpc.UpdateMoveStateSendRpc( MobHash, transform.position, Destination );
            }
        }
    }
}