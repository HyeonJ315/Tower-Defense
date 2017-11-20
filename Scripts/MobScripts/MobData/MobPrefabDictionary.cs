using System;
using System.Collections.Generic;
using Assets.Scripts.TurretScripts.TurretData;

namespace Assets.Scripts.MobScripts.MobData
{
    public enum MobPrefabs
    {
        Invalid = -1,
        TestMob = 0,
    }

    class MobPrefabDictionary
    {   // Stores key-value pairs of all mobs. the key is the string value of the prefab 
        // and the value is the index that the prefab is placed in the NetworkManager prefab.
        public static string MobPrefabsDirectory = "Prefabs/Monsters";
        public static readonly List<string> MobPrefabList = new List<string>( Enum.GetNames( typeof( TurretPrefabs ) ) );
    }
}
