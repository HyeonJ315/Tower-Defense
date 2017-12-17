using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.PlayerInputs.MouseScripts;
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
        private const string RightArrowName   = "RightArrow";
        private const string LeftArrowName    = "LeftArrow";
        private const string ResetIconName    = "Reset";
        private const string ButtonToggleName = "SlotWindowTitle";
        private const float    HideYPos  = -432.5f;
        private const float    ShowYPos  = -107.5f;
        private const float  DragSpeed = 2000;
        private bool _hidden;

        private readonly List<SlotButton> _buttons = new List<SlotButton>(18);

        private List<List<SlotButtonStruct>> _buttonStructStack;
        private int _currentIndex;

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
                switch (button.name)
                {
                    case ButtonToggleName:
                        button.onClick.AddListener( ToggleVisibility );
                        break;
                    case RightArrowName:
                        button.onClick.AddListener( _slideRight );
                        break;
                    case LeftArrowName:
                        button.onClick.AddListener( _slideLeft );
                        break;
                    case ResetIconName:
                        button.onClick.AddListener( _resetButton );
                        break;
                }
            }
            SlotDescription.Instance.DisableActiveState();
            _buttonStructStack = new List< List< SlotButtonStruct > >();
            _resetButton();
        }

        protected void OnGUI()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _resetButton();
            }
        }

        private void _slideRight()
        {
            if ( _buttonStructStack.Count == 0 ) return;
            if (_currentIndex + ButtonCount >= _buttonStructStack.Last().Count ) return;
            _currentIndex += ButtonCount;
            _bindButtons( _buttonStructStack.Last().GetRange( _currentIndex, _buttonStructStack.Last().Count - _currentIndex ) );
        }

        private void _slideLeft()
        {
            if ( _buttonStructStack.Count == 0   ) return;
            if ( _currentIndex - ButtonCount < 0 ) return;
            _currentIndex -= ButtonCount;
            _bindButtons( _buttonStructStack.Last().GetRange( _currentIndex, _buttonStructStack.Last().Count - _currentIndex ) );
        }

        private void _resetButton()
        {
            if ( _buttonStructStack.Count <= 1 ) return;
            _buttonStructStack.RemoveAt( _buttonStructStack.Count - 1 );
            var buttonStructs = _buttonStructStack.Last();
            _currentIndex = 0;
            _bindButtons( buttonStructs.GetRange( _currentIndex, buttonStructs.Count - _currentIndex ) );
            MouseStateManager.Instance.EnableMouseDragSelecter();
        }

        public void SubscribeButtonStructs( List<SlotButtonStruct> buttonStructs = null, bool pushToStack = false )
        {
            if ( buttonStructs == null || buttonStructs.Count == 0 )
            {
                _unbindButtons();
                return;
            }
            if( !pushToStack ) _buttonStructStack = new List<List<SlotButtonStruct>>();
            _buttonStructStack.Add( buttonStructs );
            _currentIndex = 0;
            _bindButtons( buttonStructs.GetRange( _currentIndex, buttonStructs.Count - _currentIndex ) );
        }

        private void _bindButtons( List<SlotButtonStruct> buttonStructs )
        {
            foreach( var buttonStruct in buttonStructs )
            {
                if ( buttonStruct.ActionList.Count == 0 )
                {
                    Debug.Log( "Could not bind button because there was no action." );
                    return;
                }
                if ( buttonStruct.ImagePrefab == null )
                {
                    Debug.Log("Could not bind button because there was no image.");
                    return;
                }
            }
            _unbindButtons();
            for (var i = 0; i < buttonStructs.Count && i < ButtonCount; i++)
            {
                if( buttonStructs[i] == null ) continue;
                _bindButton( i, buttonStructs[i] );
            }
        }

        private void _unbindButtons()
        {
            for (var i = 0; i < _buttons.Count; i++)
            {
                _unbindButton( i );
            }
        }

        public void ToggleVisibility()
        {
            _hidden = !_hidden;
        }


        private void _bindButton(int buttonId, SlotButtonStruct buttonStruct)
        {
            if (buttonId >= ButtonCount || buttonId < 0) return;
            if (buttonStruct.ActionList.Count == 0)
            {
                Debug.Log("Could not bind button because there was no action.");
                return;
            }
            if (buttonStruct.ImagePrefab == null)
            {
                Debug.Log("Could not bind button because there was no image.");
                return;
            }
            _unbindButton(buttonId);
            _systematicBind(_buttons[buttonId], buttonStruct);
        }

        private static void _systematicBind(SlotButton button, SlotButtonStruct buttonStruct)
        {
            button.onClick.RemoveAllListeners();
            foreach (var action in buttonStruct.ActionList)
                button.onClick.AddListener(action);
            button.Description = buttonStruct.Description;
            button.Bound = true;
            var image = Instantiate( buttonStruct.ImagePrefab );
            var buttonTransform = image.transform;
            buttonTransform.SetParent(button.transform);
            buttonTransform.localPosition = Vector3.zero;
            buttonTransform.localScale = Vector3.one;
            buttonTransform.SetAsFirstSibling();
            image.name = ButtonIconName;
        }

        private void _unbindButton(int buttonId)
        {
            if (buttonId >= ButtonCount || buttonId < 0) return;
            _buttons[buttonId].onClick.RemoveAllListeners();
            _buttons[buttonId].Bound = false;
            var buttonTransform = _buttons[buttonId].transform.Find(ButtonIconName);
            if (buttonTransform) Destroy( buttonTransform.gameObject );
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