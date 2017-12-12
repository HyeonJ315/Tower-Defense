using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerInputs.SlotScripts
{
    public class SlotDescription : MonoBehaviour
    {
        #region Singleton

        public static SlotDescription Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        private const string DescriptionText = "DescriptionText";
        public Text TextComponent;

        public void Start()
        {
            TextComponent = transform.Find(DescriptionText).GetComponent<Text>();
        }

        public void ToggleActiveState()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        public void DisableActiveState()
        {
            gameObject.SetActive( false );
        }

        public void EnableActiveState()
        {
            gameObject.SetActive( true );
        }
    }
}
