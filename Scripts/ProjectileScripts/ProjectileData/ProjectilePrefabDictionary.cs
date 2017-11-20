using System;
using System.Collections.Generic;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    public enum ProjectilePrefabs
    {
        Invalid = -1,
        Projectile_Fireball = 0,
    }

    class ProjectilePrefabDictionary
    {   // Stores key-value pairs of all mobs. the key is the integer ID value of the prefab 
        // the value is the prefab's name.
        public static string ProjectilePrefabsDiectory = "Prefabs/Projectiles";
        public static readonly List<string> TurretPrefabList = new List<string>(Enum.GetNames(typeof(ProjectilePrefabs)));
    }
}
