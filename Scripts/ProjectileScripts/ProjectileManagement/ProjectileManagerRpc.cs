using Assets.Scripts.AI.Platform;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.ProjectileScripts.ProjectileManagement
{
    public abstract class ProjectileManagerRpc : NetworkBehaviour
    {

        private ProjectileActuator _projectileActuator;

        protected virtual void Start()
        {
            _projectileActuator = ProjectileActuator.Instance;
        }

        public abstract void ProjectileSpawnSendRpc( Vector3 position, int projectileNumber, uint target );

        protected bool ProjectileSpawn( Vector3 position, int projectileNumber, uint target )
        {
            if ( !_projectileActuator )
            {
                _projectileActuator = ProjectileActuator.Instance;
                Debug.LogWarning("ProjectileActuator is not found for this scene!");
                return false;
            }

            var turret = MapPlatform.Instance.GetTile( position ).TurretGameObject;
            if (turret == null)
            {
                Debug.Log( "Turret is not on top of the spawning projectile." );
                return false;
            }
            var turretAttributes = turret.GetComponent<Turret>().TurretAttributes;

            GameObject projectile;
            var projectileSpawned = _projectileActuator.ProjectileSpawn( turretAttributes, position, target, out projectile );

            return projectileSpawned;
        }
    }
}