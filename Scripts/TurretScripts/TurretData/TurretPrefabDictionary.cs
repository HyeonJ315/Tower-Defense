using System;
using System.Collections.Generic;

namespace Assets.Scripts.TurretScripts.TurretData
{
    public enum TurretPrefabs
    {
        Invalid           = -1,
        Beginner_Dark0    = 0,
        Beginner_Earth0   = 1,
        Beginner_Fire0    = 2,
        Beginner_Ice0     = 3,
        Beginner_Light0   = 4,
        Beginner_Poison0  = 5,
        Beginner_Thunder0 = 6,
        Beginner_Water0   = 7,
    }
    class TurretPrefabDictionary
    {   // Stores key-value pairs of all mobs. the key is the integer ID value of the prefab 
        // the value is the prefab's name.
        public static string TurretPrefabsDirectory = "Prefabs/Turrets";
        public static readonly List<string> TurretPrefabList = new List<string>( Enum.GetNames(typeof(TurretPrefabs) ) );
    }
}
