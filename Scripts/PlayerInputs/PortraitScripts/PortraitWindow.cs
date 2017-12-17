using System;
using System.Collections.Generic;
using Assets.Scripts.PlayerInputs.SlotScripts;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.PlayerInputs.PortraitScripts
{
    public class PortraitWindow : MonoBehaviour
    {
        #region Singleton

        public static PortraitWindow Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        private const int ButtonCount = 18;
        private const string GridTag = "GridCell";
        private const string ButtonIconName = "ButtonIcon";
        private readonly List<SlotButton> _buttons = new List<SlotButton>(18);

        protected void Start()
        {
            for (var i = 0; i < ButtonCount; i++) _buttons.Add(null);
            var buttons = GetComponentsInChildren<SlotButton>();
            foreach (var button in buttons)
            {
                var tmpArray = button.name.Split('_');
                if (tmpArray.Length != 2) continue;
                if (tmpArray[0] != GridTag) continue;
                _buttons[Convert.ToInt32(tmpArray[1])] = button;
            }
        }

        private static void _systematicBind(SlotButton button, SlotButtonStruct buttonStruct)
        {
            button.onClick.RemoveAllListeners();
            foreach (var action in buttonStruct.ActionList)
                button.onClick.AddListener(action);
            button.Description = buttonStruct.Description;
            buttonStruct.ImagePrefab.transform.SetParent(button.transform);
            buttonStruct.ImagePrefab.transform.localPosition = Vector3.zero;
            buttonStruct.ImagePrefab.name = ButtonIconName;
        }

        public void BindButton(int buttonId, SlotButtonStruct buttonStruct)
        {
            if (buttonId >= ButtonCount || buttonId < 0) return;
            if (buttonStruct.ActionList.Count == 0)
            {
                Debug.Log("Could not bind button because there was no action.");
                return;
            }
            else if (buttonStruct.ImagePrefab)
            {
                Debug.Log("Could not bind button because there was no image.");
                return;
            }
            UnbindButton(buttonId);
            _systematicBind(_buttons[buttonId], buttonStruct);
        }

        public void UnbindButton(int buttonId)
        {
            if (buttonId >= ButtonCount || buttonId < 0) return;
            _buttons[buttonId].onClick.RemoveAllListeners();
            Destroy(_buttons[buttonId].transform.Find(ButtonIconName));
        }

        public void BindButtons(List<SlotButtonStruct> buttonStructs)
        {

            foreach (var buttonStruct in buttonStructs)
            {
                if (buttonStruct.ActionList.Count == 0)
                {
                    Debug.Log("Could not bind button because there was no action.");
                    return;
                }
                else if ( !buttonStruct.ImagePrefab )
                {
                    Debug.Log("Could not bind button because there was no image.");
                    return;
                }
            }
            UnbindButtons();
            for (var i = 0; i < buttonStructs.Count && i < ButtonCount; i++)
                _systematicBind(_buttons[i], buttonStructs[i]);
        }

        public void UnbindButtons()
        {
            foreach (var button in _buttons)
            {
                button.onClick.RemoveAllListeners();
                Destroy(button.transform.Find(ButtonIconName));
            }
        }
    }
}