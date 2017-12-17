using UnityEngine;

namespace Assets.Scripts.PlayerInputs.PortraitScripts
{
    public class PortraitDescription : MonoBehaviour
    {
        public void ToggleActiveState()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
