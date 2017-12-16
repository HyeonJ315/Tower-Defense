using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.RTSCamera;
using Assets.Scripts.TurretScripts.TurretData;
using Assets.Scripts.TurretScripts.TurretManagement;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs.MouseScripts
{
    class MouseClickBuilder : MonoBehaviour
    {
        [HideInInspector]
        public TurretAttributes BuildingTurret;

        private TurretAttributes _currentBuildingTurret;
        public string CameraName = "RtsCamera";
        private TurretManagerRpc _turretManagerRpc;
        private TurretActuator   _turretActuator;
        private Camera           _camera;
        private int _teamGroup = 2;
        protected void Start()
        {
            _turretActuator = TurretActuator.Instance;
            _turretManagerRpc = TurretManagerRpcClient.Instance;
            GameObject cameraGo;
            RTS_Camera.CameraDictionary.TryGetValue(CameraName, out cameraGo);
            if (cameraGo != null) _camera = cameraGo.GetComponent<Camera>();
        }

        public void Update()
        {
            if (MouseStateManager.Instance.MouseOnButton) return;

            if (!_turretManagerRpc)
            {
                _turretManagerRpc = TurretManagerRpcClient.Instance;
                return;
            }

            if (!_turretActuator)
            {
                _turretActuator = TurretActuator.Instance;
            }

            if ( _currentBuildingTurret != BuildingTurret )
            {
                _turretActuator.RemoveTurretShadow();
                _currentBuildingTurret = BuildingTurret;
            }
            RaycastHit hit;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if ( !Physics.Raycast(ray, out hit, Mathf.Infinity) ) return;
            if (hit.transform.tag != "Platform" && hit.transform.tag != "Turret") return;
            _turretActuator.ShowTurretShadow( BuildingTurret.Name, _teamGroup, hit.point);

            if (!Input.GetMouseButtonDown(0)) return;

            if (hit.transform.tag == "Platform")
            {
                var turretNumber = TurretRepository.Instance.NameToIndex[ BuildingTurret.Name ];
                _turretManagerRpc.TurretSpawnSendRpc( turretNumber, _teamGroup, hit.point );
            }
            else
            {
                Debug.Log(" Cannot deploy here. ");
            }
        }

        public void OnDisable()
        {
            if( _turretActuator) _turretActuator.RemoveTurretShadow();
        }
    }
}
