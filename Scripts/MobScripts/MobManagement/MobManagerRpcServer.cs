using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.MobScripts.MobManagement
{
    internal class MobManagerRpcServer : MobManagerRpc
    {
        #region Singleton 

        public static MobManagerRpcServer Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        public override void MobSpawnSendRpc(int mobNumber, int teamGroup, uint hashNumber = 0, int playerNumber = 0 )
        {
            RpcMobSpawn( mobNumber, teamGroup, _currentMobNumber - 2, playerNumber );
        }

        public override void MobSpawnFailedSendRpc( NetworkConnection target )
        {
            TargetMobSpawnFailed( target );
        }

        [ClientRpc]
        private void RpcMobSpawn( int mobNumber, int teamGroup, uint hashNumber, int playerNumber )
        {
            MobSpawn( mobNumber, playerNumber, teamGroup, hashNumber );
        }

        [TargetRpc]
        private void TargetMobSpawnFailed( NetworkConnection target )
        {
            MobSpawnFailed();
        }
    }
}
