using System.Collections.Generic;
using System.Net;
using Assets.Scripts.MobScripts.MobManagement;
using Assets.Scripts.TurretScripts.TurretManagement;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.NetworkManagement
{
    public class NetworkingManager : NetworkManager
    {
        public string PrefabsResourceDirectory = "Prefabs/Network";
        public bool LocalHosting = false;

        //removes all client's gameobject on disconnect.
        private readonly Dictionary< int, List<GameObject> > _clientObjectDictionary = new Dictionary< int, List<GameObject> >();

        #region Singleton

        public static NetworkingManager Instance { get; private set; }

        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        public bool IsServer { private set; get; }
        public bool IsClient { private set; get; }

        protected void Start()
        {
            DontDestroyOnLoad( gameObject );
        }

        public void StartClient( string ipAddress, ushort portNumber )
        {
            networkAddress = ipAddress;
            networkPort    = portNumber;

            StartClient();
        }

        public override void OnStartClient(NetworkClient nc)
        {
            base.OnStartClient( nc );
            if (LocalHosting)
            {
                NetworkingDiscovery.Instance.Initialize();
                NetworkingDiscovery.Instance.StartAsClient();
            }
        }

        public override void OnClientConnect(NetworkConnection networkConnection)
        {
            base.OnClientConnect( networkConnection );
            IsServer = false;
            IsClient = true;

            SceneManager.sceneLoaded += OnSceneLoaded;
            var currSceneIndex = SceneManager.GetActiveScene().buildIndex;
            var nextSceneIndex = currSceneIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }

        public void StartServer( ushort portNumber, int maxAllowedClients )
        {
            networkPort    = portNumber;
            maxConnections = maxAllowedClients;
            if (LocalHosting)
            {
                NetworkingDiscovery.Instance.Initialize();
                NetworkingDiscovery.Instance.StartAsServer();
            }
            StartServer();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            IsServer = true;
            IsClient = false;

            // For local hosting. ( debugging )

            SceneManager.sceneLoaded += OnSceneLoaded;
            var currSceneIndex = SceneManager.GetActiveScene().buildIndex;
            var nextSceneIndex = currSceneIndex + 1;
            SceneManager.LoadScene( nextSceneIndex );
        }

        public override void OnServerReady( NetworkConnection networkConnection )
        {
            base.OnServerReady( networkConnection );
            var mobManagerRpcClient = Instantiate( Resources.Load( PrefabsResourceDirectory + "/" + "MobManagerRpcClient") ) as GameObject;
            var turretManagerRpcClient = Instantiate( Resources.Load( PrefabsResourceDirectory + "/" + "TurretManagerRpcClient" ) ) as GameObject;
            var clientGameObjects = new List<GameObject> { mobManagerRpcClient, turretManagerRpcClient };
            _clientObjectDictionary.Add( networkConnection.connectionId, clientGameObjects );

            NetworkServer.SpawnWithClientAuthority( mobManagerRpcClient, networkConnection );
            NetworkServer.SpawnWithClientAuthority( turretManagerRpcClient, networkConnection );
        }

        public override void OnStopServer()
        {
            NetworkingDiscovery.Instance.StopAllCoroutines();
            NetworkingDiscovery.Instance.StopBroadcast();
        }

        public override void OnServerDisconnect( NetworkConnection networkConnection )
        {
            base.OnServerDisconnect( networkConnection );
            List<GameObject> clientGameObjects;
            if( !_clientObjectDictionary.TryGetValue( networkConnection.connectionId, out clientGameObjects ) ) return;
            foreach (var go in clientGameObjects)
                NetworkServer.Destroy( go );

            _clientObjectDictionary.Remove( networkConnection.connectionId );
        }

        public void OnSceneLoaded( Scene scene, LoadSceneMode loadSceneMode )
        {
            if( !IsClient )
                NetworkServer.SpawnObjects();
        }
    }
}
