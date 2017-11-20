using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.MobScripts.MobManagement
{
    internal class MobModifierRpcServer : MobModifierRpc
    {
        #region Singleton 

        public static MobModifierRpcServer Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        public override void UpdateMoveStateSendRpc( uint mobHashNumber, Vector3 position, Vector3 destination )
        {
            RpcUpdateMoveState( mobHashNumber, position, destination  );
        }

        public override void UpdateHealthSendRpc( uint mobHashNumber, float health )
        {
            RpcUpdateHealth( mobHashNumber, health );
        }

        [ClientRpc]
        private void RpcUpdateMoveState( uint mobHashNumber, Vector3 position, Vector3 destination )
        {
            UpdateMoveState( mobHashNumber, position, destination );
        }

        [ClientRpc]
        private void RpcUpdateHealth( uint mobHashNumber, float health )
        {
            UpdateHealth( mobHashNumber, health );
        }
    }
}
