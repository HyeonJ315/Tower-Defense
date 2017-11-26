using UnityEngine.Networking;
using UnityEngine;
namespace Assets.Scripts.PlayerInputs
{
    internal class PlayerState : NetworkBehaviour
    {
        public static PlayerState Instance { get; private set; }

        public override void OnStartAuthority()
        {
            Instance = this;
        }
    }
}
