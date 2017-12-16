using System;
using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.NetworkManagement;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.MobScripts.MobManagement
{
    public abstract class MobManagerRpc : NetworkBehaviour
    {
        protected static uint CurrentMobNumber = 1;
        private MobActuator _mobActuator;

        protected virtual void Start()
        {
            _mobActuator = MobActuator.Instance;
        }

        #region Sending rpc helper methods

        public abstract void MobSpawnSendRpc( int mobNumber, int teamGroup, uint hashNumber = 0, int playerNumber = 0 );
        public abstract void MobSpawnFailedSendRpc( NetworkConnection target );

        #endregion

        #region Mob spawn and reply failed rpc

        protected bool MobSpawn( int mobNumber, int playerNumber, int teamGroup, uint mobHashNumber = 0 )
        {
            if ( !_mobActuator )
            {
                _mobActuator = MobActuator.Instance;
                Debug.LogWarning( "MobActuator is not found for this scene!" );
                return false;
            }

            if (mobNumber < 0 || mobNumber >= MobRepository.Instance.MobCount)
                return false;

            // Spawn the network object here.
            #region Check if the mob number is defined in the mob prefabs.

            var mobName = MobRepository.Instance.IndexToName[ mobNumber ];

            #endregion

            GameObject mob1Go, mob2Go = null;
            #region Attempt to spawn the mob.

            var mobsSpawned = false;
            switch (teamGroup)
            {
                case 1:
                    mobsSpawned = _mobActuator.MobSpawn( mobNumber, playerNumber, teamGroup, new Vector3( 0, 0.5f,  14 ), "2_SpawnA to 2_MidA", out mob1Go ) &&
                                  _mobActuator.MobSpawn( mobNumber, playerNumber, teamGroup, new Vector3( 0, 0.5f, -14 ), "2_SpawnB to 2_MidB", out mob2Go ) ;
                    break;
                case 2:
                    mobsSpawned = _mobActuator.MobSpawn( mobNumber, playerNumber, teamGroup, new Vector3( 0, 0.5f,  14 ), "1_SpawnA to 1_MidA", out mob1Go ) &&
                                  _mobActuator.MobSpawn( mobNumber, playerNumber, teamGroup, new Vector3( 0, 0.5f, -14 ), "1_SpawnB to 1_MidB", out mob2Go ) ;
                    break;
                default:
                    return false;
            }

            #endregion

            #region Mob spawn failed handler.
            if ( !mobsSpawned )
            {
                if ( mob1Go )
                    Destroy( mob1Go );
                if ( mob2Go )
                    Destroy( mob2Go );
                return false;
            }

            #endregion

            var mobPos1 = mob1Go.GetComponent<Mob>();
            var mobPos2 = mob2Go.GetComponent<Mob>();

            #region Set the mob number.
            // Only usable by the server. if a client sends this request, it does not go through ( see the partial networkobject class below )
            if ( isServer )
            {
                mobPos1.MobHash = CurrentMobNumber++;
                if (CurrentMobNumber == 0) CurrentMobNumber++;
                mobPos2.MobHash = CurrentMobNumber++;
                if (CurrentMobNumber == 0) CurrentMobNumber++;
                return true;
            }
            else
            {
                if (mobHashNumber == 0) mobHashNumber++;
                mobPos1.MobHash = mobHashNumber++;
                if (mobHashNumber == 0) mobHashNumber++;
                mobPos2.MobHash = mobHashNumber;
                return true;
            }

            #endregion

        }

        protected void MobSpawnFailed()
        {
            Debug.Log( "Spawning mob failed." );
        }
        #endregion

    }
}