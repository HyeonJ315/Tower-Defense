using Assets.Scripts.NetworkManagement;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.TurretScripts.TurretManagement
{
    internal class TurretManagerRpcClient : TurretManagerRpc
    {
        #region Singleton

        public static TurretManagerRpcClient Instance { get; private set; }

        public override void OnStartAuthority()
        {
            Instance = this;
        }

        #endregion

        public override void TurretRemoveSendRpc( int teamGroup, Vector3 location )
        {
            CmdTurretRemove( teamGroup, location );
        }

        public override void TurretRemoveFailedSendRpc( NetworkConnection target )
        {
            Debug.Log( "Not Required." );
        }

        public override void TurretSpawnSendRpc( int turretNumber, int teamGroup, Vector3 location, int playerNumber )
        {
            CmdTurretSpawn(turretNumber, teamGroup, location);
        }

        public override void TurretSpawnFailedSendRpc( NetworkConnection target )
        {
            Debug.Log( "Not Required." );
        }

        [Command]
        private void CmdTurretRemove( int teamGroup, Vector3 location )
        {
            var sendingConnection = GetComponent<NetworkIdentity>().clientAuthorityOwner;
            if ( TurretRemove(teamGroup, sendingConnection.connectionId, location) )
                TurretManagerRpcServer.Instance.TurretRemoveSendRpc(teamGroup, location);
            else
            {
                TurretManagerRpcServer.Instance.TurretRemoveFailedSendRpc( sendingConnection );
            }
        }

        [Command]
        private void CmdTurretSpawn( int turretNumber, int teamGroup, Vector3 location )
        {
            var sendingConnection = GetComponent<NetworkIdentity>().clientAuthorityOwner;
            if (TurretSpawn(turretNumber, sendingConnection.connectionId, teamGroup, location))
                TurretManagerRpcServer.Instance.TurretSpawnSendRpc(turretNumber, teamGroup, location);
            else
            {
                TurretManagerRpcServer.Instance.TurretSpawnFailedSendRpc(sendingConnection);
            }
        }
    }
}   
