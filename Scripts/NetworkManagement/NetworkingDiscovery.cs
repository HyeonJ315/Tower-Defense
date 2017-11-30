using UnityEngine.Networking;

namespace Assets.Scripts.NetworkManagement
{
    public class NetworkingDiscovery : NetworkDiscovery
    {
        #region Singleton

        public static NetworkingDiscovery Instance { get; private set; }
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
            DontDestroyOnLoad( gameObject );
        }

        public override void OnReceivedBroadcast( string fromAddress, string data )
        {
            base.OnReceivedBroadcast( fromAddress, data );
            NetworkManager.singleton.networkAddress = fromAddress;
        }
    }
}