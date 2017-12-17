
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.NetworkManagement
{
    internal class GeneralRpcServer : NetworkBehaviour
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

        protected void Start()
        {
            var parent = GameObject.Find("ServerRPCs");
            if (parent) transform.SetParent(parent.transform);
        }

        [TargetRpc]
        public void TargetResponseWithPlayerNumber( NetworkConnection target, int playerNumber )
        {
            GeneralRpcClient.Instance.PlayerNumber = playerNumber;
            GeneralRpcClient.Instance.PlayerNumberSet = true;
        }
    }
}
