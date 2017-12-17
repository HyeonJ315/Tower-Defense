using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ElementScripts;
using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.MobScripts.MobManagement;
using Assets.Scripts.PlayerInputs.MouseScripts;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.PlayerInputs.SlotScripts
{
    internal class SlotSubscriber : MonoBehaviour
    {
        private bool _initialized = false;
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

        protected void Start()
        {
            _slotWindow = SlotWindow.Instance;
        }

        protected void Update()
        {
            if (_initialized) return;
            BindBuildAndSpawn();
            _initialized = true;
        }

        public void BindBuildAndSpawn()
        {
            var buildIconPrefab = MiscRepository.Instance.BuildIconPrefab;
            var spawnIconPrefab = MiscRepository.Instance.SpawnIconPrefab;
            var buildSlotButton = new SlotButtonStruct( new List<UnityAction> { BindBuildCategories }, buildIconPrefab, "Build Towers"   );
            var spawnSlotButton = new SlotButtonStruct( new List<UnityAction> { BindSpawnCategories }, spawnIconPrefab, "Spawn Monsters" );
            _slotWindow.SubscribeButtonStructs( new List<SlotButtonStruct> { buildSlotButton, spawnSlotButton } );
        }

        public void BindBuildCategories()
        {
            var slotButtonStructList = new List<SlotButtonStruct>();
            foreach ( var elementAttribute in ElementRepository.Instance.ElementAttributesList )
            {
                var attribute = elementAttribute;
                slotButtonStructList.Add( new SlotButtonStruct(
                    new List<UnityAction> { delegate { BindBuildTurrets(attribute); } },
                    elementAttribute.Icon,
                    "Build " + elementAttribute.Name + " Towers."));
            }
            if (slotButtonStructList.Count == 0) return;
            _slotWindow.SubscribeButtonStructs( slotButtonStructList, true );
        }

        public void BindBuildTurrets( ElementAttributes elementAttribute )
        {
            var turretList = TurretRepository.Instance.TurretTypeList[ elementAttribute.Index ];
            var slotButtonStructList = new List<SlotButtonStruct>();
            foreach (var turretAttribute in turretList)
            {
                var attribute = turretAttribute;
                slotButtonStructList.Add( new SlotButtonStruct(
                    new List<UnityAction> { delegate { BuildingTurret(attribute); } },
                    turretAttribute.Icon,
                    "Build " + turretAttribute.Name + "." ) );
            }
            if (slotButtonStructList.Count == 0) return;
            _slotWindow.SubscribeButtonStructs( slotButtonStructList, true );
        }

        public void BuildingTurret( TurretAttributes turretAttribute )
        {
            MouseStateManager.Instance.EnableMouseClickBuilder( turretAttribute );
        }

        public void BindSpawnCategories()
        {
            var slotButtonStructList = new List<SlotButtonStruct>();
            foreach ( var elementAttribute in ElementRepository.Instance.ElementAttributesList )
            {
                var attribute = elementAttribute;
                slotButtonStructList.Add( new SlotButtonStruct(
                    new List<UnityAction> { delegate { BindSpawnMonsters(attribute); } },
                    elementAttribute.Icon,
                    "Spawn " + elementAttribute.Name + " Monsters."));
            }
            if (slotButtonStructList.Count == 0) return;
            _slotWindow.SubscribeButtonStructs( slotButtonStructList, true );
        }

        public void BindSpawnMonsters( ElementAttributes elementAttribute )
        {
            var mobList = MobRepository.Instance.MobTypeList[elementAttribute.Index];
            var slotButtonStructList = new List<SlotButtonStruct>();
            foreach (var mobAttribute in mobList)
            {
                var attribute = mobAttribute;
                slotButtonStructList.Add(new SlotButtonStruct(
                    new List<UnityAction> { delegate { SpawningMob(attribute); } },
                    mobAttribute.Icon,
                    "Spawn " + mobAttribute.Name + "."));
            }
            if (slotButtonStructList.Count == 0) return;
            _slotWindow.SubscribeButtonStructs( slotButtonStructList, true );
        }

        public void SpawningMob(MobAttributes attribute)
        {
            MobManagerRpcClient.Instance.MobSpawnSendRpc( attribute.Index, 1 );
        }
    }
}
