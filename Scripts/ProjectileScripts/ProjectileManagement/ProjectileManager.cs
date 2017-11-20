using System;
using Assets.Scripts.ProjectileScripts.ProjectileData;
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

        public bool ProjectileSpawn( int projectileNumber, Vector3 position, uint targetNumber, out GameObject projectileGameObject )
        {
            projectileGameObject = null;
            #region create projectile

            if ( _projectileHierarchyGameObject == null )
            {
                _projectileHierarchyGameObject = new GameObject { name = "Projectiles" };
            }

            if ( !Enum.IsDefined(typeof (ProjectilePrefabs), projectileNumber) )
                return false;
            var projectileName = Enum.GetName( typeof(ProjectilePrefabs), projectileNumber );
            projectileGameObject =
                Instantiate( Resources.Load( ProjectilePrefabDictionary.ProjectilePrefabsDiectory + "/" + projectileName ) ) as GameObject;
            if (projectileGameObject == null) return false;
            projectileGameObject.transform.position = position;
            projectileGameObject.transform.SetParent( _projectileHierarchyGameObject.transform );
            
            #endregion

            var projectile = projectileGameObject.GetComponent<Projectile>();

            #region Set the projectile's attributes

            projectileGameObject.name = projectileName;
            var projectileAttributes = projectileGameObject.GetComponent<ProjectileAttributes>();
            projectileAttributes.TargetNumber = targetNumber;

            #endregion

            return true;
        }
    }
}
