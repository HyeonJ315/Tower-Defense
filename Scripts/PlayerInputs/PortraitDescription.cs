using UnityEngine;

namespace Assets.Scripts.PlayerInputs
{
    public class PortraitDescription : MonoBehaviour
    {
        public void ToggleActiveState()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
