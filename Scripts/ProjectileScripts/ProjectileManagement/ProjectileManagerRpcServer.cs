using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.ProjectileScripts.ProjectileManagement
{
    internal class ProjectileManagerRpcServer : ProjectileManagerRpc
    {
        #region Singleton

        public static ProjectileManagerRpcServer Instance { get; private set; }

        protected override void Start()
        {
            base.Start();
            transform.SetParent( GameObject.Find("ServerRPCs").transform );
        }

        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        public override void ProjectileSpawnSendRpc(Vector3 position, int projectileNumber, uint target)
        {
            RpcProjectileSpawn( position, projectileNumber, target );
        }


        [ClientRpc]
        private void RpcProjectileSpawn(Vector3 position, int projectileNumber, uint target)
        {
            ProjectileSpawn( position, projectileNumber, target );
        }
    }
}
