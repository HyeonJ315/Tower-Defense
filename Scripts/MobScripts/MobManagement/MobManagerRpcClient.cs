using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.MobScripts.MobManagement
{
    internal class MobManagerRpcClient : MobManagerRpc
    {
        #region Authoratiative mobDictionary

        public static MobManagerRpcClient Instance { get; private set; }
        public override void OnStartAuthority()
        {
            Instance = this;
        }

        #endregion

        protected override void Start()
        {
            base.Start();
            var parent = GameObject.Find("ServerRPCs");
            if (parent) transform.SetParent(parent.transform);
        }

        public override void MobSpawnSendRpc(int mobNumber, int teamGroup, uint hashNumber = 0, int playerNumber = 0 )
        {
            CmdMobSpawn( mobNumber, hashNumber );
        }

        public override void MobSpawnFailedSendRpc( NetworkConnection target )
        {
            Debug.Log("Not Implemented.");
        }

        [Command]
        private void CmdMobSpawn( int mobNumber, uint hashNumber )
        {
            var teamGroup = 1;
            var sendingConnection = GetComponent<NetworkIdentity>().clientAuthorityOwner;
            if ( MobSpawn( mobNumber, sendingConnection.connectionId, teamGroup, hashNumber ) )
                MobManagerRpcServer.Instance.MobSpawnSendRpc( mobNumber, teamGroup, hashNumber, sendingConnection.connectionId );
            else
                MobManagerRpcServer.Instance.MobSpawnFailedSendRpc( sendingConnection );
        }
    }
}
