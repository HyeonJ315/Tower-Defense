using System;
using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.ProjectileScripts.ProjectileData;
using Assets.Scripts.TrackingDictionaries;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileManagement
{
    public class ProjectileActuator : MonoBehaviour
    {
        #region Singleton

        public static ProjectileActuator Instance { get; private set; }

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

            if (turretAttributes.ProjectileNumber < 0 || 
                turretAttributes.ProjectileNumber >= ProjectileRepository.Instance.ProjectileCount)
                return false;
            var projectileName = ProjectileRepository.Instance.IndexToName[ turretAttributes.ProjectileNumber ];
            
            projectileGameObject =
                Instantiate( ProjectileRepository.Instance.ProjectileAttributesList[ turretAttributes.ProjectileNumber ].Prefab );
            if (projectileGameObject == null) return false;
            projectileGameObject.name = projectileName;
            projectileGameObject.transform.position = location + turretAttributes.ProjectileSpawnOffset;
            projectileGameObject.transform.SetParent( _projectileHierarchyGameObject.transform );
            var projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.ProjectileAttributes = new ProjectileAttributes( turretAttributes );

            GameObject targetGo;
            if ( !MobTrackerDictionary.Instance.GetEntry(targetNumber, out targetGo) )
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
