using System;
using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.ProjectileScripts.ProjectileData;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileManagement
{
    public class ProjectileManager : MonoBehaviour
    {
        #region Singleton

        public static ProjectileManager Instance { get; private set; }

        private GameObject _projectileHierarchyGameObject;
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        public bool ProjectileSpawn( TurretAttributes turretAttributes, Vector3 location, uint targetNumber,  out GameObject projectileGameObject )
        {
            projectileGameObject = null;
            #region create projectile

            if ( _projectileHierarchyGameObject == null )
            {
                _projectileHierarchyGameObject = new GameObject { name = "Projectiles" };
            }

            string projectileName;

            if (!ProjectileDictionary.Instance.ProjectileIdToName.TryGetValue( turretAttributes.ProjectileNumber, out projectileName))
                return false;
            projectileGameObject =
                Instantiate( Resources.Load( ProjectileDictionary.ProjectileDir + "/" + turretAttributes.ProjectileNumber + "_" + projectileName + "/" + projectileName ) ) as GameObject;
            if (projectileGameObject == null) return false;
            projectileGameObject.name = projectileName;
            projectileGameObject.transform.position = location + turretAttributes.ProjectileSpawnOffset;
            projectileGameObject.transform.SetParent( _projectileHierarchyGameObject.transform );
            var projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.ProjectileAttributes = new ProjectileAttributes( turretAttributes );

            GameObject targetGo;
            if ( !MobTrackerDictionary.Instance.TryGetValue(targetNumber, out targetGo) )
            {
                Destroy( projectileGameObject );
                Debug.Log( "Could not find target " + targetNumber );
                return false;
            }
            projectile.Target = targetGo.GetComponent<Mob>();

            #endregion

            return true;
        }
    }
}
