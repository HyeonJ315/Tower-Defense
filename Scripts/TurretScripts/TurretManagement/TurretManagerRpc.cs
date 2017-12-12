using Assets.Scripts.NetworkManagement;
using UnityEngine;
using UnityEngine.Networking;

// Sends RPCs to handle creation of turrets over the network such that each client
// is synchronized with the turrets built by other clients.
namespace Assets.Scripts.TurretScripts.TurretManagement
{
    public abstract class TurretManagerRpc : NetworkBehaviour
    {
        private TurretActuator _turretActuator;

        protected virtual void Start()
        {
            _turretActuator = TurretActuator.Instance;
        }

        #region Sending RPC helper methods

        public abstract void TurretRemoveSendRpc( int teamGroup, Vector3 location );
        public abstract void TurretRemoveFailedSendRpc( NetworkConnection target );
        public abstract void TurretSpawnSendRpc( int turretNumber, int teamGroup, Vector3 location, int playerNumber = 0 );
        public abstract void TurretSpawnFailedSendRpc( NetworkConnection target );

        #endregion

        #region Turret remove and reply failed RPC

        protected bool TurretRemove( int teamGroup, int playerNumber, Vector3 location )
        {
            if (!_turretActuator)
            {
                _turretActuator = TurretActuator.Instance;
                Debug.LogWarning("TurretActuator is not found for this scene!");
                return false;
            }

            var turretRemoved = _turretActuator.RemoveTurret( playerNumber.ToString(), teamGroup, location );
            
            return turretRemoved;
        }

        protected void TurretRemoveFailed()
        {
            //if (networkObject.IsServer) return;
            //TODO: For the client, maybe flash a text that says that the player has no turret there.
            Debug.Log( "Failed to remove turret." );
        }

        #endregion

        #region Turret Spawn and reply failed RPC 

        protected bool TurretSpawn( int turretNumber, int playerNumber, int teamGroup, Vector3 location )
        {   
            if (!_turretActuator)
            {
                _turretActuator = TurretActuator.Instance;
                Debug.LogWarning( "TurretActuator is not found for this scene!" );
                return false;
            }

            var turretSpawned = _turretActuator.PlaceTurret( turretNumber, playerNumber, teamGroup, location);

            return turretSpawned;
        }

        protected void TurretSpawnFailed()
        {
            if ( NetworkingManager.Instance.IsServer ) return;
            //TODO: For the client, maybe flash a text that says that the player cannot build there.
            Debug.Log( "Failed to spawn turret." );
        }

        #endregion
    }
}