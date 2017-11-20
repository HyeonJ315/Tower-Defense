using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.ProjectileScripts.ProjectileManagement
{
    public abstract class ProjectileManagerRpc : NetworkBehaviour
    {

        private ProjectileManager _projectileManager;

        protected void Start()
        {
            _projectileManager = ProjectileManager.Instance;
        }

        public abstract void ProjectileSpawnSendRpc( Vector3 position, int projectileNumber, uint target );

        protected bool ProjectileSpawn( Vector3 position, int projectileNumber, uint target )
        {
            if ( !_projectileManager )
            {
                _projectileManager = ProjectileManager.Instance;
                Debug.LogWarning("ProjectileManager is not found for this scene!");
                return false;
            }

            GameObject projectile;
            var projectileSpawned = _projectileManager.ProjectileSpawn( projectileNumber, position, target, out projectile );

            return projectileSpawned;
        }
    }
}