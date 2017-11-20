using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    // Mobs place their game objects as an index number and projectiles are able to find the object through index.
    internal class MobTrackerDictionary
    {
        public static readonly Dictionary<uint, GameObject> Instance = new Dictionary<uint, GameObject>();

        public static bool InsertMobEntry(uint index, GameObject gameObject)
        {
            GameObject go;
            if ( Instance.TryGetValue(index, out go) )
                return false;
            Instance.Add( index, gameObject );
            return true;
        }

        public static bool DeleteMobEntry(uint index)
        {
            GameObject go;
            if (!Instance.TryGetValue(index, out go))
                return false;
            Instance.Remove( index );
            return true;
        }
    }
}
