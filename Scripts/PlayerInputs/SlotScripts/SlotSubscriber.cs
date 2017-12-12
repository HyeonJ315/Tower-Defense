using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ElementScripts;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.PlayerInputs.SlotScripts
{
    internal class SlotSubscriber : MonoBehaviour
    {
        #region Singleton

        public static SlotSubscriber Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        private SlotWindow _slotWindow;

        private const string BuildIconResourceLocation = "UserInterface/BuildIcon";
        private const string SpawnIconResourceLocation = "UserInterface/SpawnIcon";

        protected void Start()
        {
            _slotWindow = SlotWindow.Instance;
        }

        protected void Update()
        {
            if( Input.GetKeyDown("l") )
            BindBuildAndSpawn();
        }

        public void BindBuildAndSpawn()
        {
            var buildIcon = Instantiate( Resources.Load( BuildIconResourceLocation ) ) as GameObject;
            var spawnIcon = Instantiate( Resources.Load( SpawnIconResourceLocation ) ) as GameObject;
            var buildSlotButton = new SlotButtonStruct( new List<UnityAction> { BindBuildCategories }, buildIcon, "BindBuildTurrets Towers" );
            var spawnSlotButton = new SlotButtonStruct( new List<UnityAction> { BindSpawn           }, spawnIcon, "Spawn Monsters"          );
            _slotWindow.BindButtons( new List<SlotButtonStruct> { buildSlotButton, spawnSlotButton } );
        }

        public void BindBuildCategories()
        {
            var slotButtonStructList = new List<SlotButtonStruct>();
            foreach ( var elementAttribute in ElementRepo.Instance.ElementAttributesList )
            {
                var elementIcon = Instantiate(elementAttribute.Icon);
                var attribute = elementAttribute;
                slotButtonStructList.Add( new SlotButtonStruct(
                    new List<UnityAction> { delegate { BindBuildTurrets(attribute); } },
                    elementIcon,
                    "Build " + elementAttribute.Name + " Towers."));
            }
            _slotWindow.BindButtons( slotButtonStructList );
        }

        public void BindBuildTurrets( ElementAttributes elementAttribute )
        {
            Debug.Log( elementAttribute.Name );
        }

        public void BindSpawn()
        {
            Debug.Log( "SPAWN!" );
        }

    }
}
