using System.Linq;
using Assets.Scripts.NetworkManagement;
using Assets.Scripts.RTSCamera;
using Assets.Scripts.TurretScripts.TurretManagement;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs
{
    public class GuiSelling : Gui
    {
        private TurretManagerRpc _turretManagerRpc;
        private Camera _camera;
        public string CameraName = "RtsCamera";
        protected override void Start ()
        {
            _turretManagerRpc = TurretManagerRpcClient.Instance;
            GameObject cameraGo;
            RTS_Camera.CameraDictionary.TryGetValue(CameraName, out cameraGo);
            _camera = cameraGo.GetComponent<Camera>();
            SetButton("SelectButton", Button_Back, "");
        }

        protected override void Update ()
        {
            if (!_turretManagerRpc)
            {
                _turretManagerRpc = TurretManagerRpcClient.Instance;;
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out hit, Mathf.Infinity)) return;
                if (hit.transform.tag != "Platform") return;
                if (TargetsTags.Any(t => hit.transform.CompareTag(t)))
                {
                    _turretManagerRpc.TurretRemoveSendRpc( TeamGroup, hit.point );
                }
            }

            if ( Input.GetKeyDown("1") )
                Button_Back("");
        }

        private void Button_Back(string msg)
        {
            ReplaceMeWith("GUI_Idle");
        }
    }
}
