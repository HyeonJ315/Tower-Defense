using System;
using System.Collections.Generic;
using Assets.Scripts.Queue;
using UnityEngine;

namespace Assets.Scripts.AI.Platform
{
    // Y is up. X and Z are the platform dimensions.
    public class MapPlatform : MonoBehaviour
    {
        #region Singleton

        public static MapPlatform Instance { get; private set; }

        #endregion
        // This variable is used to tag tiles for tracking tile data states.
        // ( think about finding shortest path multiple times with dirty tile data variables )
        private uint _currentTag;

        private const int   DiagWeight = 14;
        private const int   LineWeight = 10;
        private const float Epsilon    = 0.0001f;

        public int Height       = 0; // Z
        public int Width        = 0; // X
        public int TilesPerUnit = 2;
        private int _height; // Z
        private int _width;  // X
        private int _tpu;
        private float _offset;
        // 0 ___
        //   ___
        //   ___ W * H * 4
        // Last Index ends at negative X,Z from the center.
        private List<TileData> _tileList; // Index 0 starts at positive X,Z from the center. 
        private Vector3 _topLeft; // ( +X, +Z )

        protected void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _tpu    = TilesPerUnit;
            _height = Height;
            _width  = Width;
            _offset = 0.5f / _tpu;
            _tileList = new List<TileData>();
            if (_height <= 0) Debug.LogWarning("_height dimension not set for platform: " + gameObject.name);
            if (_width  <= 0) Debug.LogWarning("_width dimension not set for platform: " + gameObject.name);
            _topLeft = gameObject.transform.position + new Vector3(_width, 0, _height)/ 2;
            for (var zz = 0; zz < _height*_tpu; ++zz)
            {
                for (var xx = 0; xx < _width*_tpu; ++xx)
                {
                    _tileList.Add(new TileData(_getTilePosition(xx, zz), _getTileIndex(xx, zz)));
                }
            }
            AutoDetectWalls();
        }

        #region Private Helper Methods.

        // sets the HCost of the start tile index.
        private void _setHCost(int startTileIndex, int endTileIndex)
        {
            int xEnd, zEnd, xStart, zStart;
            _getXZFromTileIndex( startTileIndex, out xStart, out zStart );
            _getXZFromTileIndex( endTileIndex  , out xEnd  , out zEnd   );
            if (xEnd == -1 || zEnd == -1)
            {
                Debug.LogWarning("HCost was not calculated for this combination: " + startTileIndex + " " + endTileIndex);
                return;
            }
            var xDiff = Math.Abs(xEnd - xStart);
            var zDiff = Math.Abs(zEnd - zStart);
            int diagCount;
            int lineCount;
            if (xDiff > zDiff)
            {
                diagCount = zDiff;
                lineCount = xDiff - zDiff;
            }
            else
            {
                diagCount = xDiff;
                lineCount = zDiff - xDiff;
            }
            _tileList[startTileIndex].HCost = DiagWeight*diagCount + LineWeight*lineCount;
        }

        private int _getTileIndexFromLocation(Vector3 location)
        {
            var fromTopLeft = ( (location - _topLeft) )* _tpu;
            var index = _getTileIndex( Mathf.Abs( fromTopLeft.x ), Mathf.Abs( fromTopLeft.z ) );
            return index;
        }

        private void _getXZFromTileIndex(int tileIndex, out int x, out int z)
        {
            if (tileIndex < 0 || tileIndex >= _tileList.Count)
            {
                x = -1;
                z = -1;
            }
            else
            {
                z = tileIndex/(_width*_tpu);
                x = tileIndex%(_width*_tpu);
            }
        }

        private int _getTileIndex(int x, int z)
        {
            if ( x < 0 || x >= _width*_tpu  ) return -1;
            if ( z < 0 || z >= _height*_tpu ) return -1;
            return _width*_tpu*z + x;
        }

        private int _getClippedTileIndexFromLocation(Vector3 location)
        {
            var fromTopLeft = ( ( _topLeft - location ) ) * _tpu;
            var index = _getTileIndexClipped( fromTopLeft.x, fromTopLeft.z );
            return index;
        }

        private int _getTileIndexClipped(int x, int z)
        {   //Debug.Log( "X: " + x + " Z: " + z );
            if ( x < 0             ) x = 0;
            if ( x >= _width*_tpu  ) { x = _width * _tpu - 1;}
            if ( z < 0             ) z = 0;
            if ( z >= _height*_tpu ) { z = _height * _tpu - 1;}
            return _width * _tpu * z + x;
        }

        private int _getTileIndexClipped(float x, float z)
        {
            return _getTileIndexClipped( (int) x, (int) z );
        }

        private int _getTileIndex(float x, float z)
        {
            return _getTileIndex((int) x, (int) z);
        }

        private Vector3 _getTilePosition(int x, int z)
        {
            if (x < 0 || x >= _width*_tpu)
            {
                Debug.LogWarning("Invalid Indices X: " + x + " Z: " + z);
                return Vector3.zero;
            }
            if (z < 0 || z >= _height*_tpu)
            {
                Debug.LogWarning("Invalid Indices X: " + x + " Z: " + z);
                return Vector3.zero;
            }
            return new Vector3(_topLeft.x - ((0.5f + x)/_tpu),
                _topLeft.y,
                _topLeft.z - ((0.5f + z)/_tpu));
        }

        private bool _getClosestCorners( Vector3 location, out List<Vector3> cornerList )
        {
            cornerList = new List<Vector3>();
            var index = _getTileIndexFromLocation(location);
            if (index < 0)
            {
                return false;
            }
            var centerLocation = _tileList[index].Location;

            cornerList.Add(centerLocation + new Vector3(  _offset, 0,  _offset ) );
            cornerList.Add(centerLocation + new Vector3( -_offset, 0,  _offset ) );
            cornerList.Add(centerLocation + new Vector3(  _offset, 0, -_offset ) );
            cornerList.Add(centerLocation + new Vector3( -_offset, 0, -_offset ) );

            return true;
        }

        #endregion

        #region Public Methods.

        public bool LocationIsBlocked( Vector3 location )
        {
            var index = _getTileIndexFromLocation(location);
            return index >= 0 && _tileList[index].Blocked;
        }

        // Detect all walls upon awakening
        public void AutoDetectWalls()
        {
            for (var zz = 0; zz < _height*_tpu; ++zz)
            {
                for (var xx = 0; xx < _width*_tpu; ++xx)
                {
                    RaycastHit outHit;
                    var blockedTileData = _tileList[_getTileIndex(xx, zz)];
                    var currentTilePos = blockedTileData.Location;
                    if (Physics.Raycast(currentTilePos, Vector3.up, out outHit, Mathf.Infinity,
                        LayerMask.GetMask("Wall")))
                    {
                        blockedTileData.Blocked = true;
                    }
                    else if (Physics.Raycast(currentTilePos, Vector3.up, out outHit, Mathf.Infinity,
                        LayerMask.GetMask("Team1Area")))
                    {
                        blockedTileData.Team1Area = true;
                    }
                    else if (Physics.Raycast(currentTilePos, Vector3.up, out outHit, Mathf.Infinity,
                        LayerMask.GetMask("Team2Area")))
                    {
                        blockedTileData.Team2Area = true;
                    }
                }
            }
        }

        public TileData GetTile( Vector3 pos )
        {
            var index = _getTileIndexFromLocation( pos );
            return index > 0 ? _tileList[ index ] : null;
        }

        // obtain the turret tiles from one location with the name tag.
        public bool GetTurretTiles2X2(Vector3 location, out List<TileData> turretTiles2X2)
        {
            turretTiles2X2 = new List<TileData>();
            var index = _getTileIndexFromLocation( location );

            var firstTileData = _tileList[index];
            if (firstTileData.TurretTag.Length == 0) return false;
            turretTiles2X2.Add( firstTileData );
            var turretTileStart = firstTileData.TurretPos;

            switch (turretTileStart)
            {
                case "X+Z+":
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3( -2*_offset, 0, -2*_offset ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3( -2*_offset, 0, 0 ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3( 0, 0, -2*_offset ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    break;
                case "X-Z+":
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3(  2*_offset, 0, -2*_offset ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3(  2*_offset, 0, 0 ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3( 0, 0, -2*_offset ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    break;
                case "X+Z-":
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3( -2*_offset, 0,  2*_offset ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3( -2*_offset, 0, 0 ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3( 0, 0,  2*_offset ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    break;
                case "X-Z-":
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3(  2*_offset, 0,  2*_offset ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3(  2*_offset, 0, 0 ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    index = _getTileIndexFromLocation( firstTileData.Location + new Vector3( 0, 0,  2*_offset ) );
                    turretTiles2X2.Add( _tileList[ index ] );
                    break;
            }
            return turretTiles2X2.Count >= 4;
        }

        public bool GetClosest2X2( Vector3 location, out List<TileData> closest2X2, out Vector3 closestCorner )
        {
            closest2X2        = new List<TileData>();
            closestCorner     = Vector3.zero;
            var closest2X2Tmp = new List<TileData>();
            List<Vector3> cornerList;

            if( !_getClosestCorners(location, out cornerList) )
            {
                return false;
            }
            // does the corner have 4 tiles touching it?
            var validCornerList = new List<Vector3>();
            foreach ( var corner in cornerList)
            {
                int index1, index2, index3, index4;
                if( (index1 = _getTileIndexFromLocation( corner + new Vector3(  Epsilon, 0,  Epsilon ) ) ) < 0 ) continue;
                if( (index2 = _getTileIndexFromLocation( corner + new Vector3( -Epsilon, 0,  Epsilon ) ) ) < 0 ) continue;
                if( (index3 = _getTileIndexFromLocation( corner + new Vector3(  Epsilon, 0, -Epsilon ) ) ) < 0 ) continue;
                if( (index4 = _getTileIndexFromLocation( corner + new Vector3( -Epsilon, 0, -Epsilon ) ) ) < 0 ) continue;
                closest2X2Tmp.Add( _tileList[index1] );
                closest2X2Tmp.Add( _tileList[index2] );
                closest2X2Tmp.Add( _tileList[index3] );
                closest2X2Tmp.Add( _tileList[index4] );
                validCornerList.Add( corner );
            }
            if ( validCornerList.Count == 0 )
            {
                return false;
            }

            // from the list of valid corners, take the closest distance to the location.
            var closestDistance = Vector3.Distance( location, validCornerList[0] );
            var closestIndex = 0;
            for ( var it = 1; it < validCornerList.Count; ++it )
            {   var testDistance = Vector3.Distance( location, validCornerList[it] );
                if (!(testDistance < closestDistance)) continue;
                closestDistance = testDistance;
                closestIndex = it;
            }

            // return the four corners and the closest corner.
            closest2X2.Add( closest2X2Tmp[ closestIndex * 4 + 0 ] );
            closest2X2.Add( closest2X2Tmp[ closestIndex * 4 + 1 ] );
            closest2X2.Add( closest2X2Tmp[ closestIndex * 4 + 2 ] );
            closest2X2.Add( closest2X2Tmp[ closestIndex * 4 + 3 ] );
            closestCorner = validCornerList[ closestIndex ];
            return true;
        }

        public bool GetTilesInRadius( Vector3 location, float radius, out List<TileData> tilesInRadius )
        {
            List<TileData> closest2X2;
            Vector3 closestCorner;
            tilesInRadius = new List<TileData>();
            if ( !GetClosest2X2(location, out closest2X2, out closestCorner) )
            {
                return false;
            }
            
            // Locate a square group to limit search. to that subset of tiles.
            // TODO: implement Depth First Search...?
            var posIndex = _getClippedTileIndexFromLocation( location + new Vector3( +radius, 0, +radius ) + new Vector3( -Epsilon, 0, -Epsilon ) );
            var negIndex = _getClippedTileIndexFromLocation( location + new Vector3( -radius, 0, -radius ) + new Vector3( +Epsilon, 0, +Epsilon ) );
            if ( posIndex < 0 || negIndex < 0 )
            {
                return false;
            }
            int posX, posZ, negX, negZ;
            _getXZFromTileIndex( negIndex, out posX, out posZ );
            _getXZFromTileIndex( posIndex, out negX, out negZ );
            for ( var zz = negZ; zz <= posZ; zz++ )
            {
                for ( var xx = negX; xx <= posX; xx++ )
                {   
                    var currTile = _tileList[ _getTileIndex( xx, zz ) ];
                    if (Vector3.Distance( currTile.Location, closestCorner) < radius)
                    {
                        tilesInRadius.Add( currTile );
                    }
                }
            }
            return true;
        }

        // A* shortest path algorithm inputs 2 locations, outputs a compact and detailed solution.
        public void FindAStarPath(Vector3 start, Vector3 end, out List<Vector3> compact, out List<Vector3> detailed, 
            bool disableTeam1Tiles = false,
            bool disableTeam2Tiles = false)
        {
           
            var detailedReturn = new List< Vector3 >();
            var compactReturn  = new List< Vector3 >();

            var startIndex = _getTileIndexFromLocation(start);
            var destinationIndex = _getTileIndexFromLocation(end);

            #region basic platform checks and trivial check

            if (startIndex < 0)
            {
                Debug.LogWarning("Starting location did not exist within the platform: " + start );
                detailed = detailedReturn;
                compact  = compactReturn;
                return;
            }
            if (destinationIndex < 0)
            {
                Debug.LogWarning("Ending location did not exist within the platform: " + end );
                detailed = detailedReturn;
                compact  = compactReturn;
                return;
            }
            if (startIndex == destinationIndex)
            {
                detailed = detailedReturn;
                compact  = compactReturn;
                return;
            }

            #endregion

            var closedSet = new Dictionary<int, TileData>();
            var openSetPq = new PriorityQueue<TileData>();

            #region setup the starting node for a*

            // setup the start tile.
            _currentTag++;
            var startTile = _tileList[startIndex];
            _setHCost(startTile.TileIndex, destinationIndex);
            startTile.Tag = _currentTag;
            startTile.GCost = 0;
            startTile.ParentTileIndex = startTile.TileIndex;
            startTile.FCost = startTile.HCost + startTile.GCost;
            openSetPq.Enqueue(startTile);

            #endregion

            #region perform a* shortest path algorithm ( with forced neighbors )

            while (openSetPq.Count() != 0)
            {
                var nextTileData = openSetPq.Peek();
                openSetPq.Dequeue();
                
                closedSet.Add( nextTileData.TileIndex, nextTileData );

                var destinationFound = false;
                for (var zz = -1; zz <= 1; ++zz )
                {
                    for (var xx = -1; xx <= 1; ++xx )
                    {
                        if (xx == 0 && zz == 0) continue;

                        #region adjacent tiles validity checks

                        int nextX, nextZ;
                        _getXZFromTileIndex(nextTileData.TileIndex, out nextX, out nextZ);
                        var extraOpenTileIndex = (_width * _tpu * zz + xx) + nextTileData.TileIndex;

                        // is the tile index within boundaries?
                        if (nextX + xx < 0 || nextX + xx >= _width*_tpu)
                        {
                            continue;
                        }
                        if (nextZ + zz < 0 || nextZ + zz >= _height*_tpu)
                        {
                            continue;
                        }

                        // is the tile data blocked?
                        var extraTileData = _tileList[extraOpenTileIndex];
                        if (extraTileData.Blocked)
                        {
                            continue;
                        }
                        // is the tile data not in the closed set?
                        TileData tmp;
                        if (closedSet.TryGetValue(extraOpenTileIndex, out tmp))
                        {
                            continue;
                        }


                        // if checking a diagonal, are adjacent tiles blocked?
                        if (xx == 1 && zz == 1)
                        {
                            var adjacentTileIndex1 = (_width*_tpu*(zz - 1) + (xx - 0)) + nextTileData.TileIndex;
                            var adjacentTileIndex2 = (_width*_tpu*(zz - 0) + (xx - 1)) + nextTileData.TileIndex;
                            if ( _tileList[adjacentTileIndex1].Blocked || _tileList[adjacentTileIndex2].Blocked )
                                continue;
                        }
                        // if checking a diagonal, are adjacent tiles blocked?
                        else if (xx == -1 && zz == 1)
                        {
                            var adjacentTileIndex1 = (_width*_tpu*(zz - 1) + (xx + 0)) + nextTileData.TileIndex;
                            var adjacentTileIndex2 = (_width*_tpu*(zz - 0) + (xx + 1)) + nextTileData.TileIndex;
                            if ( _tileList[adjacentTileIndex1].Blocked || _tileList[adjacentTileIndex2].Blocked )
                                continue;
                        }
                        // if checking a diagonal, are adjacent tiles blocked?
                        else if (xx == 1 && zz == -1)
                        {
                            var adjacentTileIndex1 = (_width*_tpu*(zz + 1) + (xx - 0)) + nextTileData.TileIndex;
                            var adjacentTileIndex2 = (_width*_tpu*(zz + 0) + (xx - 1)) + nextTileData.TileIndex;
                            if ( _tileList[adjacentTileIndex1].Blocked || _tileList[adjacentTileIndex2].Blocked )
                                continue;
                        }
                        // if checking a diagonal, are adjacent tiles blocked?
                        else if (xx == -1 && zz == -1)
                        {
                            var adjacentTileIndex1 = (_width*_tpu*(zz + 1) + (xx + 0)) + nextTileData.TileIndex;
                            var adjacentTileIndex2 = (_width*_tpu*(zz + 0) + (xx + 1)) + nextTileData.TileIndex;
                            if ( _tileList[adjacentTileIndex1].Blocked || _tileList[adjacentTileIndex2].Blocked )
                                continue;
                        }

                        // are team 1 tiles blocked?
                        if (disableTeam1Tiles && extraTileData.Team1Area)
                        {
                            continue;
                        }

                        // are team 2 tiles blocked?
                        if (disableTeam2Tiles && extraTileData.Team2Area)
                        {
                            continue;
                        }

                        #endregion

                        #region Forced Neighbors validity checks

                        // Forced Neighbors validity checks
                        if ( nextTileData.ParentTileIndex >= 0 && nextTileData.TileIndex != startIndex )
                        {
                            int prevX, prevZ;
                            _getXZFromTileIndex(nextTileData.ParentTileIndex, out prevX, out prevZ);
                            var stateX = nextX - prevX;
                            var stateZ = nextZ - prevZ;
                            // Diagonals
                            if (Math.Abs(stateX) == 1 && Math.Abs(stateZ) == 1)
                            {
                                #region handle diagonal parents

                                if (xx == 0 && zz == -stateZ)      continue;
                                if (xx == -stateX && zz == 0)      continue;
                                if (xx == stateX && zz == -stateZ) continue;
                                if (xx == -stateX && zz == stateZ) continue;

                                #endregion
                            }
                            // Horizontals
                            else if ( Math.Abs(stateX) == 1 && stateZ == 0 )
                            {
                                #region handle horizontal parents

                                //  checks for the three different forced neighor cases.
                                bool blockadeExistsPos = false,
                                     blockadeExistsNeg = false;

                                var xxx = -stateX;

                                // is the tile index within boundaries? ( Negative )
                                if (nextZ - 1 >= 0 && nextZ - 1 < _height * _tpu)
                                {   var testTileIndex = (_width * _tpu * (-1) + xxx) + nextTileData.TileIndex;
                                    blockadeExistsNeg = _tileList[testTileIndex].Blocked;
                                }

                                // is the tile index within boundaries? ( positive )
                                if (nextZ + 1 >= 0 && nextZ + 1 < _height * _tpu)
                                {   var testTileIndex = (_width * _tpu * (1) + xxx) + nextTileData.TileIndex;
                                    blockadeExistsPos = _tileList[testTileIndex].Blocked;
                                }


                                // Both blocked
                                if ( blockadeExistsPos && blockadeExistsNeg )
                                {
                                    // Do nothing
                                }
                                // One blocked positive side
                                else if ( blockadeExistsPos )
                                {
                                    if ( zz == -1 ) continue;
                                }
                                // One blocked negative side
                                else if ( blockadeExistsNeg )
                                {
                                    if ( zz == 1 ) continue;
                                }
                                // neither blocked
                                else
                                {
                                    if (Math.Abs(xx) == 1 && Math.Abs(zz) == 1) { continue; }
                                    if (zz != 0) { continue; }
                                }

                                #endregion
                            }
                            // Verticals
                            else if ( stateX == 0 && Math.Abs(stateZ) == 1 )
                            {
                                #region handle vertical parents

                                //  checks for the three different forced neighor cases.
                                bool blockadeExistsPos = false,
                                     blockadeExistsNeg = false;

                                var zzz = -stateZ;

                                // is the tile index within boundaries? ( Negative )
                                if (nextX - 1 >= 0 && nextX - 1 < _width * _tpu)
                                {   var testTileIndex = (_width * _tpu * zzz - 1) + nextTileData.TileIndex;
                                    blockadeExistsNeg = _tileList[testTileIndex].Blocked;
                                }

                                // is the tile index within boundaries? ( Positive )
                                if (nextX + 1 >= 0 && nextX + 1 < _width * _tpu)
                                {   var testTileIndex = (_width * _tpu * zzz + 1) + nextTileData.TileIndex;
                                    blockadeExistsPos = _tileList[testTileIndex].Blocked;
                                }


                                // Both blocked
                                if (blockadeExistsPos && blockadeExistsNeg)
                                {
                                    // Do nothing
                                }
                                // One blocked positive side
                                else if (blockadeExistsPos)
                                {
                                    if ( xx == -1 ) continue;
                                }
                                // One blocked negative side
                                else if (blockadeExistsNeg)
                                {
                                    if ( xx == 1 ) continue;
                                }
                                // neither blocked
                                else
                                {
                                    if ( Math.Abs( xx ) == 1 && Math.Abs( zz ) == 1 ) { continue; }
                                    if ( xx != 0 ) { continue; }

                                }

                                #endregion
                            }
                        }

                        #endregion
                        
                        #region end case 

                        // is the tile data the destination?
                        if ( extraTileData.TileIndex == destinationIndex )
                        {
                                // only add vectors to return on state swaps for compact ( switching movement directions ).
                                // add all vectors to return on detailed
                                int stateX = 0, stateZ = 0;

                                int prevX, prevZ;
                                _getXZFromTileIndex(extraTileData.TileIndex, out prevX, out prevZ);

                                var prevTileData = extraTileData;
                                var currTileData = nextTileData;
                                while ( currTileData != prevTileData )
                                {
                                    int currX, currZ;
                                    _getXZFromTileIndex(currTileData.TileIndex, out currX, out currZ);

                                    int newStateX = currX - prevX,
                                        newStateZ = currZ - prevZ;
                                    if (newStateX != stateX || newStateZ != stateZ)
                                    {
                                        compactReturn.Add( prevTileData.Location );
                                    }
                                    detailedReturn.Add( prevTileData.Location );
                                    stateX = newStateX;
                                    stateZ = newStateZ;
                                    prevX = currX;
                                    prevZ = currZ;

                                    prevTileData = currTileData;
                                    currTileData = _tileList[currTileData.ParentTileIndex];
                                }
                                compactReturn.Add ( _tileList[startIndex].Location );
                                detailedReturn.Add( _tileList[startIndex].Location );

                                destinationFound = true;
                                break;
                        }

                        #endregion

                        #region continue case

                        // if the tile data is already in the open set.
                        if ( extraTileData.Tag == _currentTag )
                        {
                            var nextGCost = nextTileData.GCost +
                                            (Math.Abs(xx) == 1 && Math.Abs(zz) == 1 ? DiagWeight : LineWeight);
                            if (extraTileData.GCost <= nextGCost) continue;
                            extraTileData.GCost = nextGCost;
                            extraTileData.ParentTileIndex = nextTileData.TileIndex;
                            extraTileData.FCost = extraTileData.HCost + extraTileData.GCost;
                        }
                        // if the tile data is not in the open set.
                        else
                        {
                            _setHCost(extraTileData.TileIndex, destinationIndex);
                            extraTileData.GCost = nextTileData.GCost +
                                                  (Math.Abs(xx) == 1 && Math.Abs(zz) == 1 ? DiagWeight : LineWeight);
                            extraTileData.Tag = _currentTag;
                            extraTileData.ParentTileIndex = nextTileData.TileIndex;
                            extraTileData.FCost = extraTileData.HCost + extraTileData.GCost;
                            openSetPq.Enqueue(extraTileData);
                        }

                        #endregion
                    }
                    if (destinationFound) break;
                }
                if (destinationFound) break;
            } 

            #endregion

            if ( _currentTag != 0 )
            {
                compact  = compactReturn;
                detailed = detailedReturn;
                return;
            }

            #region tag overflow handler

            // resets the tilelist on integer overflow of _currentTag (literally 4billion calls to method findshortestpath.)
            // hey! it might happen...

            foreach (var tileData in _tileList)
            {
                tileData.HCost = 0;
                tileData.GCost = 0;
                tileData.FCost = 0;
                tileData.ParentTileIndex = -1;
                _currentTag = 0;
            }
            compact  = compactReturn;
            detailed = detailedReturn;

            #endregion
        }
        #endregion
    }

}