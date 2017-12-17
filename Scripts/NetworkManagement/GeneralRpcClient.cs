
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.NetworkManagement
{
    internal class GeneralRpcClient : NetworkBehaviour
    {
        public int PlayerNumber;
        public bool PlayerNumberSet;
        private Stopwatch _stopWatch = new Stopwatch();
        private const int PollTimer = 1000;

        #region Singleton

        public static GeneralRpcClient Instance { get; private set; }
        public override void OnStartAuthority()
        {
            Instance = this;
            CmdRequestPlayerNumber();
            _stopWatch.Stop();
            _stopWatch.Reset();
            _stopWatch.Start();
        }

        #endregion
        protected void Start()
        {
            var parent = GameObject.Find("ServerRPCs");
            if (parent) transform.SetParent(parent.transform);
        }

        [Command]
        public void CmdRequestPlayerNumber()
        {
            var sendingConnection = GetComponent<NetworkIdentity>().clientAuthorityOwner;
            GeneralRpcServer.Instance.TargetResponseWithPlayerNumber( sendingConnection, sendingConnection.connectionId );
        }

        protected void Update()
        {
            _handlePollingServer();
        }

        private void _handlePollingServer()
        {
            if ( PlayerNumber != 0 ) return;
            if ( _stopWatch != null )
            {
                _stopWatch.Stop();
                _stopWatch.Reset();
                _stopWatch = null;
                return;
            }
            if ( _stopWatch != null && _stopWatch.ElapsedMilliseconds > PollTimer )
            {
                CmdRequestPlayerNumber();
                _stopWatch.Stop();
                _stopWatch.Reset();
                _stopWatch.Start();
            }
        }
    }
}
