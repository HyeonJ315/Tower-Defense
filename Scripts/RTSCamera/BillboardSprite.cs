using UnityEngine;

namespace Assets.Scripts.RTSCamera
{
    public class BillboardSprite : MonoBehaviour
    {
        public string CameraName = "RtsCamera";
        private Transform _rtsCameraTransform;

        protected void Start()
        {
            GameObject cameraGo;
            if ( RTS_Camera.CameraDictionary.TryGetValue( CameraName, out cameraGo ) )
                _rtsCameraTransform = cameraGo.transform;
        }

        protected void Update()
        {
            var tmp = _rtsCameraTransform.forward;
            transform.LookAt( transform.position - tmp );
        }
    }
}
