using System;
using System.Collections.Generic;
using Assets.Scripts.AI.Paths;
using Assets.Scripts.AI.Platform;
using Assets.Scripts.Debugger;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretManagement
{
    public class TurretActuator : MonoBehaviour
    {
        #region Singleton

        public static TurretActuator Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion
        public Vector3 TurretShadowOffset = new Vector3( 0, 0.5f, 0 );
        public GameObject MapPlatformGameObject;
        public GameObject PathGroupTeam1;
        public GameObject PathGroupTeam2;
        public GameObject Debugger;

        private MapPlatform _mapPlatform;
        private PathGroup _pathGroupTeam1;
        private PathGroup _pathGroupTeam2;
        //private MyDebugger _myDebugger;

        private GameObject _turretsHierarchyGameObject;
        private GameObject _team1TurretsGameObject;
        private GameObject _team2TurretsGameObject;

        private GameObject _currentTurretShadow;
        private List< GameObject > _currentShadowTiles2X2; 
        public string ShadowTile = "ShadowTile";

        private readonly Color _blockedTileColor = new Color( 1.0f, 0.3f, 0.3f, 0.75f );
        private readonly Color _openTileColor    = new Color( 0.3f, 1.0f, 0.3f, 0.75f );
        private const string ShadowTag = "Shadow_";

        protected void Start()
        {
            _mapPlatform = MapPlatformGameObject.GetComponent<MapPlatform>();
            _pathGroupTeam1 = PathGroupTeam1.GetComponent<PathGroup>();
            _pathGroupTeam2 = PathGroupTeam2.GetComponent<PathGroup>();
            _currentShadowTiles2X2 = new List<GameObject>();
            //if( Debugger ) _myDebugger = Debugger.GetComponent<MyDebugger>();
            Instance = this;
        }

        // Places a turret with prefab turretName at the location for the player name in the team group.
        public bool PlaceTurret( int turretNumber, int playerNumber, int teamGroup, Vector3 location )
        {   // prefabs.
            List< TileData > closestTiles2X2;
            Vector3 closestCorner;

            #region Perform basic sanity checks

            if( turretNumber < 0 || turretNumber >= TurretRepository.Instance.TurretCount )
            {
                Debug.Log( turretNumber );
                return false;
            }

            if (teamGroup != 1 && teamGroup != 2)
            {
                return false;
            }

            if ( !_mapPlatform.GetClosest2X2( location, out closestTiles2X2, out closestCorner ) )
            {
                return false;
            }

            foreach ( var tile in closestTiles2X2 )
            {
                if (tile.Blocked)
                {
                    return false;
                }
                if ( teamGroup == 1 && !tile.Team1Area )
                {
                    return false;
                }
                if ( teamGroup == 2 && !tile.Team2Area )
                {
                    return false;
                }
            }

            #endregion

            #region Block each tile that the turret is on top of.

            foreach (var tile in closestTiles2X2)
            {
                tile.Blocked = true; 
            }

            #endregion

            var turretName = TurretRepository.Instance.IndexToName[ turretNumber ];
            if (_turretsHierarchyGameObject == null)
            {
                _turretsHierarchyGameObject = new GameObject() { name = "Turrets" };
            }

            GameObject go;

            #region Spawn the turret onto the world for the specific team group.

            switch ( teamGroup )
            {
                case 1:

                    if ( !_pathGroupTeam1.RecalculateAllPaths() )
                    {
                        foreach (var tile in closestTiles2X2)
                        {
                            tile.Blocked = false;
                        }
                        return false;
                    }
                    if ( !_spawnTurretPrefab(closestCorner, turretNumber, turretName, playerNumber, teamGroup, out go) )
                        return false;

                    break;
                case 2:

                    if ( !_pathGroupTeam2.RecalculateAllPaths() )
                    {
                        foreach (var tile in closestTiles2X2)
                        {
                            tile.Blocked = false;
                        }
                        return false;
                    }
                    if ( !_spawnTurretPrefab(closestCorner, turretNumber, turretName, playerNumber, teamGroup, out go) )
                        return false;

                    break;

                default:
                    return false;
            }
            #endregion

            #region Label the tiles that the turret is on top of.

            foreach (var tile in closestTiles2X2)
            {
                var diff = tile.Location - closestCorner;
                tile.TurretTag = turretName + "@" + playerNumber + "@" + teamGroup;
                tile.TurretGameObject = go;
                tile.TurretTeamGroup  = teamGroup;
                tile.TurretPName      = "";
                if (diff.x > 0 && diff.z > 0)
                {
                    tile.TurretPos = "X+Z+";
                }
                else if (diff.x > 0 && diff.z < 0)
                {
                    tile.TurretPos = "X+Z-";
                }
                else if (diff.x < 0 && diff.z > 0)
                {
                    tile.TurretPos = "X-Z+";
                }
                else if (diff.x < 0 && diff.z < 0)
                {
                    tile.TurretPos = "X-Z-";
                }
            }

            #endregion

            var turretData = go.GetComponent<Turret>();

            List< TileData > tilesInRange;

            #region Add the turret's influence to each tile within the turret's region.

            _mapPlatform.GetTilesInRadius( closestCorner, turretData.TurretAttributes.Range, out tilesInRange );
            foreach ( var tile in tilesInRange )
            {
                tile.TurretZonesList.Add( turretData );
            }

            #endregion

            return true;
        }

        // removes a turret at the location for a team group.
        public bool RemoveTurret( string playerName, int teamGroup, Vector3 location )
        {
            List<TileData> turretTiles2X2;
            if (!_mapPlatform.GetTurretTiles2X2(location, out turretTiles2X2))
            {
                return false;
            }
            var turretTag       = turretTiles2X2[0].TurretTag;
            var turretObject    = turretTiles2X2[0].TurretGameObject;
            var turretTeamGroup = turretTiles2X2[0].TurretTeamGroup;
            var turretPName     = turretTiles2X2[0].TurretPName;

            #region Perform basic sanity checks

            if (teamGroup != 1 && teamGroup != 2)
            {
                return false;
            }
            if ( turretTeamGroup != teamGroup )
            {
                return false;
            }
            if (turretTag == "")
            {
                return false;
            }
            if (turretObject == null)
            {
                return false;
            }
            if (turretPName != playerName)
            {
                return false;
            }
            #endregion

            Destroy(turretObject);

            var closestCorner = Vector3.zero;

            #region Calculate the center of the turret and reset the tiles it was blocking.

            foreach ( var tile in turretTiles2X2 )
            {
                tile.TurretTag   = "";
                tile.TurretPos   = "";
                tile.TurretPName = "";
                tile.TurretGameObject = null;
                tile.Blocked   = false;
                closestCorner += tile.Location;
            }
            closestCorner = closestCorner/4;

            #endregion

            #region Recalculate all paths for the specific team group.

            switch( teamGroup )
            {
                case 1:
                    _pathGroupTeam1.RecalculateAllPaths();
                    break;
                case 2:
                    _pathGroupTeam2.RecalculateAllPaths();
                    break;
            }

            #endregion 

            var turretData = turretObject.GetComponent<Turret>();

            List<TileData> tilesInRange;

            #region For each tile that the turret influenced, remove the turret from its zone list.

            _mapPlatform.GetTilesInRadius( closestCorner, turretData.TurretAttributes.Range, out tilesInRange );
            foreach (var tile in tilesInRange)
            {
                tile.TurretZonesList.Remove( turretData );
            }

            #endregion

            return true;
        }

        // removes the turret's shadow of turret type turretName.
        public bool RemoveTurretShadow()
        {
            if ( _currentTurretShadow == null )
            {
                return false;                
            }
            Destroy( _currentTurretShadow );
            _currentTurretShadow = null;
            _currentShadowTiles2X2.Clear();
            return true;
        }

        // shows the turret shadow at a location with 4 tiles showing where it can and cannot be placed.
        public bool ShowTurretShadow( string turretName, int teamGroup, Vector3 location )
        {
            List<TileData> closestTiles2X2;
            Vector3 closestCorner;
            if ( !_mapPlatform.GetClosest2X2(location, out closestTiles2X2, out closestCorner) )
            {
                RemoveTurretShadow();
                return false;
            }

            int turretNumber;
            if ( !TurretRepository.Instance.NameToIndex.TryGetValue(turretName, out turretNumber ) )
            {
                return false;
            }

            // Find the turret shadow if it already exists so we are not constantly creating and deleting turret shadows.
            if ( _turretsHierarchyGameObject == null )
            {   _turretsHierarchyGameObject = new GameObject() { name = "Turrets" };
            }

            if ( _currentTurretShadow == null )
            {   
                _currentTurretShadow = Instantiate( TurretRepository.Instance.TurretAttributesList[ turretNumber ].Prefab );
                if ( _currentTurretShadow == null )
                {
                    RemoveTurretShadow();
                    return false;
                }
                _currentTurretShadow.GetComponent<Turret>().Initialize( -1, turretNumber, turretName, teamGroup );

                #region Set the shadow turret game object's hierarchy and transparancy.

                _currentTurretShadow.name = ShadowTag + turretName;
                _currentTurretShadow.transform.SetParent( _turretsHierarchyGameObject.transform );
                var spriteRenderer = _currentTurretShadow.GetComponentInChildren< SpriteRenderer >();
                var meshRenderer   = _currentTurretShadow.GetComponentInChildren< MeshRenderer   >();
                Destroy( meshRenderer.gameObject );
                var stColor = spriteRenderer.material.color; stColor.a = 0.5f;
                spriteRenderer.material.color = stColor;

                #endregion

                #region Instantiate the Shadow Tiles

                var offset = 0.5f / Vector3.Distance(closestTiles2X2[0].Location, closestTiles2X2[1].Location);

                GameObject shadowTile;
                if( !_spawnShadowTile( _currentTurretShadow.transform, new Vector3(-offset, 0, -offset), "X-Z-", out shadowTile ) )
                    return false;
                _currentShadowTiles2X2.Add( shadowTile );

                if( !_spawnShadowTile( _currentTurretShadow.transform, new Vector3(+offset, 0, -offset), "X+Z", out shadowTile ) )
                    return false;
                _currentShadowTiles2X2.Add( shadowTile );

                if( !_spawnShadowTile( _currentTurretShadow.transform, new Vector3(-offset, 0, +offset), "X-Z+", out shadowTile ) )
                    return false;
                _currentShadowTiles2X2.Add( shadowTile );

                if( !_spawnShadowTile( _currentTurretShadow.transform, new Vector3(+offset, 0, +offset), "X+Z+", out shadowTile ) )
                    return false;
                _currentShadowTiles2X2.Add( shadowTile );

                #endregion

            }

            var newTurretShadowPos = new Vector3( closestCorner.x, 0, closestCorner.z ) + TurretShadowOffset;
            _currentTurretShadow.transform.localPosition = newTurretShadowPos;

            #region set shadow tile colors

            foreach (var tile in closestTiles2X2)
            {   // (0 -> X-Z-) (1 -> X+Z-) (2 -> X-Z+) (3 ->X+Z+)
                var diff = tile.Location - closestCorner;
                if (diff.x > 0 && diff.z > 0)
                {   
                    if (tile.Blocked || (teamGroup == 1 && !tile.Team1Area) || (teamGroup == 2 && !tile.Team2Area))
                    {
                        _currentShadowTiles2X2[3].GetComponent<MeshRenderer>().material.color = _blockedTileColor;
                    }
                    else
                    {
                        _currentShadowTiles2X2[3].GetComponent<MeshRenderer>().material.color = _openTileColor;
                    }
                }
                else if ( diff.x < 0 && diff.z > 0 )
                {
                    if (tile.Blocked || (teamGroup == 1 && !tile.Team1Area) || (teamGroup == 2 && !tile.Team2Area))
                    {
                        _currentShadowTiles2X2[2].GetComponent<MeshRenderer>().material.color = _blockedTileColor;
                    }
                    else
                    {
                        _currentShadowTiles2X2[2].GetComponent<MeshRenderer>().material.color = _openTileColor;
                    }
                }
                else if (diff.x > 0 && diff.z < 0)
                {
                    if (tile.Blocked || (teamGroup == 1 && !tile.Team1Area) || (teamGroup == 2 && !tile.Team2Area))
                    {
                        _currentShadowTiles2X2[1].GetComponent<MeshRenderer>().material.color = _blockedTileColor;
                    }
                    else
                    {
                        _currentShadowTiles2X2[1].GetComponent<MeshRenderer>().material.color = _openTileColor;
                    }
                }
                else if (diff.x < 0 && diff.z < 0)
                {
                    if (tile.Blocked || (teamGroup == 1 && !tile.Team1Area) || (teamGroup == 2 && !tile.Team2Area))
                    {
                        _currentShadowTiles2X2[0].GetComponent<MeshRenderer>().material.color = _blockedTileColor;
                    }
                    else
                    {
                        _currentShadowTiles2X2[0].GetComponent<MeshRenderer>().material.color = _openTileColor;
                    }
                }
            }

            #endregion

            return true;
        }

        private bool _spawnShadowTile( Transform parentTransform, Vector3 vec3Offset, string posTag, out GameObject shadowTile )
        {
            shadowTile = Instantiate( TurretRepository.Instance.ShadowTile );
            if (shadowTile == null)
            {
                RemoveTurretShadow();
                return false;
            }
            shadowTile.transform.SetParent( parentTransform );
            shadowTile.transform.localPosition = vec3Offset;
            shadowTile.name = ShadowTag + posTag;
            return true;
        }

        private bool _spawnTurretPrefab( Vector3 location, int turretNumber, string turretName, int playerNumber, int teamGroup, out GameObject go )
        {
            go = Instantiate( TurretRepository.Instance.TurretAttributesList[ turretNumber ].Prefab );
            if (!go) return false;

            go.GetComponent<Turret>().Initialize( playerNumber, turretNumber, turretName, teamGroup);
            go.name = turretName + "@" + playerNumber + "@" + teamGroup;
            go.tag = "Turret";
            go.transform.position = location + go.transform.position;

            if (_team1TurretsGameObject == null)
            {
                _team1TurretsGameObject = new GameObject() { name = "Team_" + teamGroup };
            }
            _team1TurretsGameObject.transform.SetParent(_turretsHierarchyGameObject.transform);
            go.transform.SetParent( _team1TurretsGameObject.transform );
            return true;
        }
    }
}
