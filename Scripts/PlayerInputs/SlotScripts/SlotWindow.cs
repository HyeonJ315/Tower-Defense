using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerInputs.SlotScripts
{
    public class SlotWindow : MonoBehaviour
    {
        #region Singleton

        public static SlotWindow Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        private const int    ButtonCount      = 18;
        private const string GridTag          = "GridCell";
        private const string ButtonIconName   = "ButtonIcon";
        private const string ButtonToggleName = "SlotWindowTitle";
        private const int    HideYPos  = -420;
        private const int    ShowYPos  = -120;
        private const float  DragSpeed = 2000;
        private bool _hidden;

        private readonly List<SlotButton> _buttons = new List<SlotButton>(18);
        public SlotButton HoveringButton;
        protected void Start()
        {
            for( var i = 0; i < ButtonCount; i++ ) _buttons.Add( null );
            var buttons = GetComponentsInChildren<SlotButton>();
            foreach (var button in buttons)
            {
                var tmpArray = button.name.Split('_');
                if ( tmpArray.Length != 2   ) continue;
                if ( tmpArray[0] != GridTag ) continue;
                _buttons[ Convert.ToInt32( tmpArray[1]) ] = button;
            }
            var allButtons = GetComponentsInChildren<Button>();
            foreach (var button in allButtons)
            {
                if ( button.name == ButtonToggleName )
                    button.onClick.AddListener(ToggleVisibility);
                break;
            }
            SlotDescription.Instance.DisableActiveState();
        }

        public void BindButton(int buttonId, SlotButtonStruct buttonStruct)
        {
            if (buttonId >= ButtonCount || buttonId < 0) return;
            if ( buttonStruct.ActionList.Count == 0 )
            {
                Debug.Log("Could not bind button because there was no action.");
                return;
            }
            if ( buttonStruct.Image == null )
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
            _buttons[buttonId].Bound = false;
            var buttonTransform = _buttons[buttonId].transform.Find(ButtonIconName);
            if( buttonTransform ) Destroy( buttonTransform.gameObject );
        }

        public void BindButtons( List<SlotButtonStruct> buttonStructs )
        {

            foreach( var buttonStruct in buttonStructs )
            {
                if ( buttonStruct.ActionList.Count == 0 )
                {
                    Debug.Log( "Could not bind button because there was no action." );
                    return;
                }
                if ( buttonStruct.Image == null )
                {
                    Debug.Log("Could not bind button because there was no image.");
                    return;
                }
            }
            UnbindButtons();
            for ( var i = 0; i < buttonStructs.Count && i < ButtonCount; i++ )
                BindButton( i, buttonStructs[i] );
        }

        public void UnbindButtons()
        {
            for (var i = 0; i < _buttons.Count; i++)
            {
                UnbindButton( i );
            }
        }

        public void ToggleVisibility()
        {
            _hidden = !_hidden;
        }

        private void _handleSlotWindowPosition()
        {
            const float tolerance = 0.0001f;
            if (_hidden)
            {
                if ( Math.Abs(transform.localPosition.y - HideYPos) < tolerance ) return;
                var tmp = transform.localPosition;
                tmp.y -= Time.fixedDeltaTime * DragSpeed;
                if (tmp.y < HideYPos) tmp.y = HideYPos;
                transform.localPosition = tmp;
            }
            else
            {
                if ( Math.Abs(transform.localPosition.y - ShowYPos) < tolerance ) return;
                var tmp = transform.localPosition;
                tmp.y += Time.fixedDeltaTime * DragSpeed;
                if (tmp.y > ShowYPos) tmp.y = ShowYPos;
                transform.localPosition = tmp;
            }
        }

        private static void _systematicBind( SlotButton button, SlotButtonStruct buttonStruct )
        {
            button.onClick.RemoveAllListeners();
            foreach (var action in buttonStruct.ActionList)
                button.onClick.AddListener(action);
            button.Description = buttonStruct.Description;
            button.Bound = true;
            buttonStruct.Image.transform.SetParent( button.transform );
            buttonStruct.Image.transform.localPosition = Vector3.zero;
            buttonStruct.Image.transform.localScale    = Vector3.one;
            buttonStruct.Image.transform.SetAsFirstSibling();
            buttonStruct.Image.name = ButtonIconName;
        }

        private void _handleDescription()
        {
            if ( _hidden )
                SlotDescription.Instance.DisableActiveState();
        }

        public void FixedUpdate()
        {
            _handleSlotWindowPosition();
            _handleDescription();
        }
    }
}