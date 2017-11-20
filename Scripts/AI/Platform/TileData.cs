using System;
using System.Collections.Generic;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;

namespace Assets.Scripts.AI.Platform
{
    // A nice little wrapper for tile data.
    public class TileData : IComparable<TileData>
    {
        public bool Team1Area;
        public bool Team2Area;

        public uint Tag;
        
        public bool Blocked;

        // If blocked, what turret is blocking the tile?
        public string TurretTag;
        public string TurretPos;
        public string TurretPName;
        public int    TurretTeamGroup;
        public GameObject TurretGameObject;

        // which turrets are monitoring this tile as an attack zone?
        public List< Turret > TurretZonesList;

        public int FCost;
        public int GCost;
        public int HCost;
        public Vector3 Location;
        public int ParentTileIndex;
        public int TileIndex;

        public TileData(Vector3 location, int tileIndex)
        {
            Tag = 0;
            TileIndex = tileIndex;
            Location = location;
            GCost = 0;
            HCost = 0;
            FCost = 0;
            ParentTileIndex = -1;
            Blocked = false;

            TurretTag   = "";
            TurretPos   = "";
            TurretPName = "";
            TurretGameObject = null;

            Team1Area = false;
            Team2Area = false;
            TurretZonesList = new List<Turret>();
        }

        public int CompareTo(TileData that)
        {
            if (FCost > that.FCost) return  1;
            if (FCost < that.FCost) return -1;
            if (HCost > that.HCost) return  1;
            if (HCost < that.HCost) return -1;
            return 0;
        }
    }
}
