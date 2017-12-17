using Assets.Scripts.NetworkManagement;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.TurretScripts.TurretManagement
{
    internal class TurretManagerRpcServer : TurretManagerRpc
    {
        #region Singleton

        public static TurretManagerRpcServer Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        protected override void Start()
        {
            base.Start();
            var parent = GameObject.Find("ServerRPCs");
            if( parent ) transform.SetParent( parent.transform );
        }

        public override void TurretRemoveSendRpc( int teamGroup, Vector3 location )
        {
            RpcTurretRemove( teamGroup, location );
        }

        public override void TurretRemoveFailedSendRpc( NetworkConnection target )
        {
            TargetTurretRemoveFailed( target );
        }

        public override void TurretSpawnSendRpc( int turretNumber, int teamGroup, Vector3 location, int playerNumber = 0 )
        {
            RpcTurretSpawn( turretNumber, teamGroup, location, playerNumber );
        }

        public override void TurretSpawnFailedSendRpc( NetworkConnection target )
        {
            TargetTurretSpawnFailed( target );
        }

        [ClientRpc]
        private void RpcTurretRemove(int teamGroup, Vector3 location)
        {
            TurretRemove( teamGroup, GeneralRpcClient.Instance.PlayerNumber, location );
        }

        [TargetRpc]
        private void TargetTurretRemoveFailed( NetworkConnection target )
        {
            TurretRemoveFailed();
        }

        [ClientRpc]
        private void RpcTurretSpawn(int turretNumber, int teamGroup, Vector3 location, int playerNumber )
        {
            
            TurretSpawn( turretNumber, playerNumber, teamGroup, location );
        }

        [TargetRpc]
        private void TargetTurretSpawnFailed( NetworkConnection target )
        {
            TurretSpawnFailed();
        }
    }
}
