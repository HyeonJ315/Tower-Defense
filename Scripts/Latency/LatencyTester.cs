/*using System.Diagnostics;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using UnityEngine;

namespace Assets.Scripts.Latency
{
    public class LatencyTester : LatencyTesterBehavior
    {
        public static Dictionary< string, GameObject > LatencyTesterDictionary = new Dictionary<string, GameObject>();
        public int PollDelay = 100;
        public int Samples   = 10;
        public double CurrentLatency { private set; get; }

        private readonly Stopwatch _stopwatch = new Stopwatch();

        // Barrel list
        private readonly List<double> _pingList = new List<double>();
        private int    _currentSamples;
        private bool   _setOnce;
        protected void Start()
        {
            _stopwatch.Start();
            LatencyTesterDictionary.Add( name, gameObject );
        }

        protected void OnDestroy()
        {
            LatencyTesterDictionary.Remove( name );
        }

        private void Networker_pingReceived( double ping )
        {
            if (_currentSamples >= Samples)
            {
                CurrentLatency = ( CurrentLatency * _currentSamples - _pingList[0] ) / ( _currentSamples - 1 );
                _pingList.RemoveAt(0);
                _currentSamples--;
            }
            CurrentLatency = ( CurrentLatency * _currentSamples + ping ) / ( _currentSamples + 1 );
            _pingList.Add( ping );
            _currentSamples++;
        }

        protected void Update()
        {
            if (networkObject == null) return;
            if ( networkObject.IsServer ) return;

            if (!_setOnce)
            {
                _setOnce = true;
                networkObject.Networker.pingReceived += Networker_pingReceived;
            }
            if ( _stopwatch.ElapsedMilliseconds < PollDelay ) return;
            _stopwatch.Stop();
            _stopwatch.Reset();
            _stopwatch.Start();
            networkObject.Networker.Ping();
        }
    }
}
*/