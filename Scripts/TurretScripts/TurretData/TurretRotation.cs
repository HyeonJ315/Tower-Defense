using Assets.Scripts.RTSCamera;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    internal abstract class TurretRotation : MonoBehaviour
    {
        public string CameraName = "RtsCamera";
        protected Animator   Animator;
        protected GameObject Camera;
        protected Turret     Turret;

        protected void Start()
        {
            Turret = transform.parent.gameObject.GetComponent<Turret>();
            Animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
            RTS_Camera.CameraDictionary.TryGetValue( CameraName, out Camera );
        }

        protected void FixedUpdate()
        {
            #region Update Idle/Attacking Sprite to face correctly.

            var v1 = transform.forward;
            var v2 = Camera.transform.forward;

            var angle = CalculateAngle( v1, v2 );
            PlayAnimation( angle );

            #endregion
        }

        protected abstract float CalculateAngle(Vector3 v1, Vector3 v2);
        protected abstract void  PlayAnimation( float angle );
    }
}
