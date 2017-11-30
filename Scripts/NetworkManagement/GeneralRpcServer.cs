
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.NetworkManagement
{
    internal class GeneralRpcServer : GeneralRpc
    {
        #region Singleton

        public static GeneralRpcServer Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        [TargetRpc]
        public void TargetResponseWithPlayerNumber( NetworkConnection target, int playerNumber )
        {
            GeneralRpcClient.Instance.PlayerNumber = playerNumber;
        }
    }
}
