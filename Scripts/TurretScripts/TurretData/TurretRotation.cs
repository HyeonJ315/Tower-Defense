using Assets.Scripts.RTSCamera;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    internal class TurretRotation : MonoBehaviour
    {
        public string CameraName = "RtsCamera";
        private Animator   _animator;
        private GameObject _camera;
        private TurretScripts.TurretData.Turret _turret;
        public string AttackSpriteFront = string.Empty;
        public string AttackSpriteLeft  = string.Empty;
        public string AttackSpriteUp    = string.Empty;
        public string AttackSpriteRight = string.Empty;

        public string IdleSpriteFront   = string.Empty;
        public string IdleSpriteLeft    = string.Empty;
        public string IdleSpriteUp      = string.Empty;
        public string IdleSpriteRight   = string.Empty;

        protected void Start()
        {
            _turret = transform.parent.gameObject.GetComponent<TurretScripts.TurretData.Turret>();
            _animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
            RTS_Camera.CameraDictionary.TryGetValue( CameraName, out _camera );
        }

        protected void FixedUpdate()
        {
            #region Update Idle/Attacking Sprite to face correctly.

            var v1 = transform.forward;
            var v2 = _camera.transform.forward;
            v1.y = v2.y = 0;
            var angle = Vector3.Angle(v1, v2);
            if( Vector3.Cross(v1, v2).y < 0 ) angle = 360.0f - angle;
            angle -= 135;
            angle = angle < 0 ? angle + 360 : angle;

            if ( angle > 0 && angle <= 90 )
            {
                _animator.Play( _turret.Attacking ? AttackSpriteFront : IdleSpriteFront );
            }
            else if (angle > 90 && angle <= 180)
            {
                _animator.Play( _turret.Attacking ? AttackSpriteLeft : IdleSpriteLeft );
            }
            else if (angle > 180 && angle <= 270)
            {
                _animator.Play( _turret.Attacking ? AttackSpriteUp : IdleSpriteUp );
            }
            else
            {
                _animator.Play( _turret.Attacking ? AttackSpriteRight : IdleSpriteRight );
            }

            #endregion
        }

    }
}
