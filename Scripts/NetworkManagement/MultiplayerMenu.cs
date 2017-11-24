using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.NetworkManagement
{
    public class MultiplayerMenu : MonoBehaviour
    {

        #region Singleton

        public static MultiplayerMenu Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        public InputField IpAddress  = null;
        public InputField PortNumber = null;

        private NetworkingManager _networkManager;

        public static int MaxAllowedClients = 4;

        private string _ipAddress;
        private ushort _portNumber;

        protected void Start()
        {
            IpAddress.text  = "127.0.0.1";
            PortNumber.text = "1337";
            _networkManager = NetworkingManager.Instance;
        }

        public void Connect()
        {
            #region Parse ip address and port number from input fields.

            _ipAddress = IpAddress.text;
            if ( !ushort.TryParse(PortNumber.text, out _portNumber) )
            {
                Debug.LogError("The supplied port number is not within the allowed range 0-" + ushort.MaxValue);
                return;
            }

            #endregion

            Debug.Log( _ipAddress + " " + _portNumber );
            _networkManager.StartClient( _ipAddress, _portNumber );
        }

        public void Host()
        {
            #region Parse ip address and port number from input fields.

            _ipAddress = IpAddress.text;

            if ( !ushort.TryParse(PortNumber.text, out _portNumber) )
            {
                Debug.LogError("The supplied port number is not within the allowed range 0-" + ushort.MaxValue);
                return;
            }

            #endregion  
            _networkManager.StartServer( _portNumber, MaxAllowedClients );
        }
    }
}
