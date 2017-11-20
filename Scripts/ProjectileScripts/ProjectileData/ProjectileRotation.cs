using Assets.Scripts.RTSCamera;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    internal class ProjectileRotation : MonoBehaviour
    {
        public string CameraName = "RTS_Camera";
        private Animator _animator;
        private GameObject _camera;
        private Transform _parentTransform;
        private ProjectileAttributes _projectileAttributes;
        public string IdleSpriteDown  = string.Empty;
        public string IdleSpriteLeft  = string.Empty;
        public string IdleSpriteUp    = string.Empty;
        public string IdleSpriteRight = string.Empty;

        public string DieSpriteDown   = string.Empty;
        public string DieSpriteLeft   = string.Empty;
        public string DieSpriteUp     = string.Empty;
        public string DieSpriteRight  = string.Empty;

        private bool _alreadyHit = false;
        protected void Start()
        {
            _animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
            _projectileAttributes = transform.parent.gameObject.GetComponent<ProjectileAttributes>();
            _parentTransform = transform.parent.transform;
            RTS_Camera.CameraDictionary.TryGetValue(CameraName, out _camera);
        }

        protected void FixedUpdate()
        {
            #region Continuously find the camera gameobject

            if (_camera == null)
            {
                RTS_Camera.CameraDictionary.TryGetValue(CameraName, out _camera);
                return;
            }

            #endregion

            #region Update Idle/Attacking Sprite to face correctly.

            var v1 = _parentTransform.forward;
            var v2 = _camera.transform.forward;
            v1.y = v2.y = 0;
            var angle = Vector3.Angle(v1, v2);
            if (Vector3.Cross(v1, v2).y < 0) angle = 360.0f - angle;
            angle -= 135;
            angle = angle < 0 ? angle + 360 : angle;

            if ( angle > 0 && angle <= 90 && !_projectileAttributes.Hit  && IdleSpriteDown != string.Empty )
            {
                _animator.Play( IdleSpriteDown );
            }
            else if ( angle > 90 && angle <= 180 && !_projectileAttributes.Hit && IdleSpriteLeft != string.Empty )
            {
                _animator.Play( IdleSpriteLeft );
            }
            else if (angle > 180 && angle <= 270 && !_projectileAttributes.Hit && IdleSpriteUp != string.Empty )
            {
                _animator.Play( IdleSpriteUp );
            }
            else if ( angle > 270 && angle <= 360 && !_projectileAttributes.Hit && IdleSpriteRight != string.Empty )
            {
                _animator.Play( IdleSpriteRight );
            }
            else if (angle > 0 && angle <= 90 && _projectileAttributes.Hit && DieSpriteDown != string.Empty && !_alreadyHit )
            {
                _animator.Play( DieSpriteDown );
                _alreadyHit = true;
            }
            else if (angle > 90 && angle <= 180 && _projectileAttributes.Hit && DieSpriteLeft != string.Empty && !_alreadyHit )
            {
                _animator.Play( DieSpriteLeft );
                _alreadyHit = true;
            }
            else if (angle > 180 && angle <= 270 && _projectileAttributes.Hit && DieSpriteUp != string.Empty && !_alreadyHit )
            {
                _animator.Play( DieSpriteUp );
                _alreadyHit = true;
            }
            else if (angle > 270 && angle <= 360 && _projectileAttributes.Hit && DieSpriteRight != string.Empty && !_alreadyHit )
            {
                _animator.Play( DieSpriteRight );
                _alreadyHit = true;
            }

            #endregion
        }

    }
}
