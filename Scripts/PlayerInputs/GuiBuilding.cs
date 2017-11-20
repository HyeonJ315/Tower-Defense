using System;
using Assets.Scripts.NetworkManagement;
using Assets.Scripts.RTSCamera;
using Assets.Scripts.TurretScripts.TurretData;
using Assets.Scripts.TurretScripts.TurretManagement;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs
{
    public class GuiBuilding : Gui
    {
        public string CurrentSpawningTurret;
        public string CameraName = "RtsCamera";
        private TurretManager    _turretManager;
        private TurretManagerRpc _turretManagerRpc;
        private Camera           _camera;

        // Use this for initialization
        protected override void Start ()
        {
            _turretManager    = TurretManager.Instance;
            _turretManagerRpc = TurretManagerRpcClient.Instance;
            GameObject cameraGo;
            RTS_Camera.CameraDictionary.TryGetValue( CameraName, out cameraGo );
            _camera = cameraGo.GetComponent<Camera>();
            SetButton( "ButtonBack", Button_Back, "" );
        }
	
        // Update is called once per frame
        protected override void Update ()
        {
            #region Find the Managers.

            if (!_turretManagerRpc)
            {
                _turretManagerRpc = TurretManagerRpcClient.Instance;
                return;
            }

            if ( !_turretManager )
            {
                _turretManager = TurretManager.Instance;
            }

            #endregion

            RaycastHit hit;

            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetKeyDown("1"))
            {
                Button_Back("");
                return;
            }

            if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
            if (hit.transform.tag != "Platform") return;
            _turretManager.ShowTurretShadow(CurrentSpawningTurret, TeamGroup, hit.point);

            if (Input.GetMouseButtonDown(0))
            {
                if( hit.transform.tag == "Platform" )
                {
                    if ( ! Enum.IsDefined( typeof(TurretPrefabs), CurrentSpawningTurret ) )
                        return;
                    var turretNumber = (TurretPrefabs)Enum.Parse(typeof(TurretPrefabs), CurrentSpawningTurret, true);
                    if( turretNumber != TurretPrefabs.Invalid )
                        _turretManagerRpc.TurretSpawnSendRpc( (int) turretNumber, TeamGroup, hit.point);
                }
            }
        }

        private void Button_Back( string msg )
        {
            _turretManager.RemoveTurretShadow();
            ReplaceMeWith("GUI_Build");
        }
    }
}
