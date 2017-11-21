using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.AI.Paths;
using Assets.Scripts.AI.Platform;
using Assets.Scripts.MobScripts.MobManagement;
using Assets.Scripts.NetworkManagement;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    public class Mob : MonoBehaviour
    {
        private bool _trackerInserted; // Is the mob placed into the tracker dictionary?
        public string MobName;
        public string PlatformName = "Platform";
        public string PathName;

        private GameObject    _pathObject; // The AStarPath wrapper game object that the mob is following in the beginning
        private AStarPath     _path;       // The Path that is on top of the game object that the mob is currently following

        private MapPlatform   _mapPlatform;    // The map platform of the current scene.

        private MobAttributesMono _mobAttributesMono;  // The mob's current attributes.

        private int           _currPathIndex;
        private List<Vector3> _currPathList = new List<Vector3>();

        public uint MobNumber    = 0;
        public int  PlayerNumber = 0;
        public int  Team         = 0;

        private Vector3 _prevLocation;

        public Vector3 Destination;
        private TileData _currentTile;

        private readonly Stopwatch _syncStopwatch = new Stopwatch();
        public int SyncUpdateInMilliseconds = 3000;
        private MobModifierRpc _mobModifierRpc;

        protected void Start()
        {
            _mobAttributesMono = GetComponents< MobAttributesMono >()[1];
            Destination   = transform.position;
            _prevLocation  = transform.position;
            _mobModifierRpc = MobModifierRpcServer.Instance;
            _mapPlatform = MapPlatform.Instance;
        }

        protected void FixedUpdate ()
        {
            _handleTileLocations();
            _handleTrackerDictionary();
            _handleMovement();
            _handleAutoSync();
        }

        protected void OnDestroy()
        {
            MobTrackerDictionary.DeleteMobEntry( MobNumber );
        }

        private void _handleTileLocations()
        {
            if (_mobAttributesMono.Dead) return;

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
            if ( MobNumber == 0 || _trackerInserted ) return;
            _trackerInserted = true;
            MobTrackerDictionary.InsertMobEntry( MobNumber, gameObject );
        }

        private void _handleMovement()
        {
            // Do not move if we do not have a path to move to!
            if ( PathName == null ) { return; } 

            // Do not move the mob if its dead. corpses dont move!
            if ( _mobAttributesMono.Dead ) { return; }

            // Do not move if not tracked.
            if ( MobNumber == 0 ) { return; }

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
                    _mobModifierRpc.UpdateMoveStateSendRpc( MobNumber, transform.position, Destination );
                    return;
                }

                #endregion

                var currPos = transform.position;
                var endPos  = _mobAttributesMono.Flying ? _currPathList[0] : Destination;
                    endPos.y = currPos.y;
                var dir = Vector3.Normalize(endPos - currPos);
                var remainingDistance = Vector3.Distance(currPos, endPos);
                var travelDistance = _mobAttributesMono.MoveSpeed * Time.fixedDeltaTime;
                var nextPosition = currPos + (dir * travelDistance);

                var distanceAvaliable = remainingDistance > travelDistance;
                var nextPosIsBlocked  = _mapPlatform.LocationIsBlocked(nextPosition);

                #region Blocked path handler.

                if (nextPosIsBlocked && !_mobAttributesMono.Flying )
                {
                    List<Vector3> placeHolder;
                    _mapPlatform.FindAStarPath( currPos, _path.EndPoint.transform.position, out _currPathList, out placeHolder );
                    _currPathIndex = _currPathList.Count - 1;
                    Destination = _currPathList[ _currPathIndex ];
                    _mobModifierRpc.UpdateMoveStateSendRpc( MobNumber, transform.position, Destination );
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
                        _mobModifierRpc.UpdateMoveStateSendRpc( MobNumber, transform.position, Destination );
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
                            _mobModifierRpc.UpdateMoveStateSendRpc( MobNumber, transform.position, Destination );
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
                var travelDistance = _mobAttributesMono.MoveSpeed * Time.fixedDeltaTime;
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
                _mobModifierRpc.UpdateMoveStateSendRpc( MobNumber, transform.position, Destination );
            }
        }
    }
}