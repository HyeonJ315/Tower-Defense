using System;
using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.NetworkManagement;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.MobScripts.MobManagement
{
    public abstract class MobManagerRpc : NetworkBehaviour
    {
        protected static uint _currentMobNumber = 1;
        private MobManager _mobManager;

        protected void Start()
        {
            _mobManager = MobManager.Instance;
        }

        #region Sending rpc helper methods

        public abstract void MobSpawnSendRpc( int mobNumber, int teamGroup, uint hashNumber = 0, int playerNumber = 0 );
        public abstract void MobSpawnFailedSendRpc( NetworkConnection target );

        #endregion

        #region Mob spawn and reply failed rpc

        protected bool MobSpawn( int mobNumber, int playerNumber, int teamGroup, uint mobHashNumber = 0 )
        {
            if ( !_mobManager )
            {
                _mobManager = MobManager.Instance;
                Debug.LogWarning( "MobManager is not found for this scene!" );
                return false;
            }

            // Spawn the network object here.
            #region Check if the mob number is defined in the mob prefabs.

            if (!MobDictionary.Instance.MobIdToName.ContainsKey(mobNumber))
                return false;

            #endregion

            GameObject mob1Go = null;
            GameObject mob2Go = null;

            #region Attempt to spawn the mob.

            var mobsSpawned = false;
            switch (teamGroup)
            {
                case 1:
                    mobsSpawned = _mobManager.MobSpawn( mobNumber, playerNumber, teamGroup, new Vector3( 0, 0.5f,  14 ), "2_SpawnA to 2_MidA", out mob1Go ) &&
                                  _mobManager.MobSpawn( mobNumber, playerNumber, teamGroup, new Vector3( 0, 0.5f, -14 ), "2_SpawnB to 2_MidB", out mob2Go ) ;

                    break;
                case 2:
                    mobsSpawned = _mobManager.MobSpawn( mobNumber, playerNumber, teamGroup, new Vector3( 0, 0.5f,  14 ), "1_SpawnA to 1_MidA", out mob1Go ) &&
                                  _mobManager.MobSpawn( mobNumber, playerNumber, teamGroup, new Vector3( 0, 0.5f, -14 ), "1_SpawnB to 1_MidB", out mob2Go ) ;
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

            var mobPos1 = mob1Go.GetComponent< Mob >();
            var mobPos2 = mob2Go.GetComponent< Mob >();

            #region Set the mob number.
            // Only usable by the server. if a client sends this request, it does not go through ( see the partial networkobject class below )
            if ( isServer )
            {
                mobPos1.MobNumber = _currentMobNumber++;
                if (_currentMobNumber == 0) _currentMobNumber++;
                mobPos2.MobNumber = _currentMobNumber++;
                if (_currentMobNumber == 0) _currentMobNumber++;
                return true;
            }
            else
            {
                if (mobHashNumber == 0) mobHashNumber++;
                mobPos1.MobNumber = mobHashNumber++;
                if (mobHashNumber == 0) mobHashNumber++;
                mobPos2.MobNumber = mobHashNumber;
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