using Assets.Scripts.RTSCamera;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    internal abstract class ProjectileRotation : MonoBehaviour
    {
        public string CameraName = "RTS_Camera";
        protected Animator   Animator;
        protected GameObject Camera;
        protected Transform  ParentTransform;
        protected ProjectileAttributes ProjectileAttributes;

        protected bool AlreadyHit;

        protected void Start()
        {
            Animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
            ProjectileAttributes = transform.parent.gameObject.GetComponent<ProjectileAttributes>();
            ParentTransform = transform.parent.transform;
            RTS_Camera.CameraDictionary.TryGetValue(CameraName, out Camera);
        }

        protected void FixedUpdate()
        {
            #region Continuously find the camera gameobject

            if (Camera == null)
            {
                RTS_Camera.CameraDictionary.TryGetValue(CameraName, out Camera);
                return;
            }

            #endregion

            #region Update Idle/Attacking Sprite to face correctly.

            var v1 = ParentTransform.forward;
            var v2 = Camera.transform.forward;
            var angle = CalculateAngle ( v1, v2 );
            PlayAnimation( angle );

            #endregion
        }

        protected abstract float CalculateAngle(Vector3 v1, Vector3 v2);
        protected abstract void PlayAnimation( float angle );
    }
}
