using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs.MouseScripts
{
    internal class MouseStateManager : MonoBehaviour
    {
        private MouseDragSelecter _mouseDragSelecter;
        private MouseClickBuilder _mouseClickBuilder;

        public bool MouseOnButton;
        #region Singleton

        public static MouseStateManager Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        protected void Start()
        {
            _mouseDragSelecter = GetComponent<MouseDragSelecter>();
            _mouseClickBuilder = GetComponent<MouseClickBuilder>();
            Disable();
        }

        public void EnableMouseDragSelecter()
        {
            _mouseClickBuilder.enabled = false;
            _mouseDragSelecter.enabled = true;
        }

        public void EnableMouseClickBuilder( TurretAttributes turretAttribute )
        {
            _mouseDragSelecter.enabled = false;
            _mouseClickBuilder.enabled = true;
            _mouseClickBuilder.BuildingTurret = turretAttribute;
        }

        public void Disable()
        {
            _mouseClickBuilder.enabled = false;
            _mouseDragSelecter.enabled = false;
        }

        protected void OnGUI()
        {
            if (Input.GetKeyDown( KeyCode.Escape) )
            {
                EnableMouseDragSelecter();
            }
        }
    }
}
