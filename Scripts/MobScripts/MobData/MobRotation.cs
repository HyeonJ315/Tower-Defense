using System.Diagnostics;
using Assets.Scripts.RTSCamera;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal class MobRotation : MonoBehaviour
    {
        public string CameraName = "RtsCamera";
        private Animator _animator;
        private GameObject _camera;
        private Transform _parentTransform;
        private MobAttributes _mobAttributes;

        public string IdleSpriteDown    = string.Empty;
        public string IdleSpriteLeft    = string.Empty;
        public string IdleSpriteUp      = string.Empty;
        public string IdleSpriteRight   = string.Empty;

        public string MoveSpriteDown    = string.Empty;
        public string MoveSpriteLeft    = string.Empty;
        public string MoveSpriteUp      = string.Empty;
        public string MoveSpriteRight   = string.Empty;

        public string DieSpriteDown     = string.Empty;
        public string DieSpriteLeft     = string.Empty;
        public string DieSpriteUp       = string.Empty;
        public string DieSpriteRight    = string.Empty;

        public string CorpseSpriteDown  = string.Empty;
        public string CorpseSpriteLeft  = string.Empty;
        public string CorpseSpriteUp    = string.Empty;
        public string CorpseSpriteRight = string.Empty;

        private Stopwatch _deathAnimationStopwatch;
        private bool _deathAnimationPlayed;
        protected void Start()
        {
            _animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
            _mobAttributes = transform.parent.gameObject.GetComponents<MobAttributes>()[1];
            _parentTransform = transform.parent.transform;
            _deathAnimationStopwatch = new Stopwatch();
            _deathAnimationStopwatch.Stop();
            _deathAnimationStopwatch.Reset();
            RTS_Camera.CameraDictionary.TryGetValue( CameraName, out _camera );
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

            if ( angle > 0 && angle <= 90 )
            {
                if ( _mobAttributes.Dead )
                {
                    if ( !_deathAnimationPlayed )
                    {
                        _animator.Play( DieSpriteDown );
                        _deathAnimationStopwatch.Start();
                        _deathAnimationPlayed = true;
                    }
                    else if ( _deathAnimationStopwatch.ElapsedMilliseconds > _mobAttributes.DeathAnimationDelay )
                    {
                        _animator.Play( CorpseSpriteDown );
                        _deathAnimationStopwatch.Stop();
                    }
                }
                else
                {
                    _animator.Play(_mobAttributes.MoveSpeed > 0 ? MoveSpriteDown : IdleSpriteDown);
                }
            }
            else if (angle > 90 && angle <= 180)
            {
                if ( _mobAttributes.Dead )
                {
                    if ( !_deathAnimationPlayed )
                    {
                        _animator.Play(DieSpriteLeft);
                        _deathAnimationStopwatch.Start();
                        _deathAnimationPlayed = true;
                    }
                    else if (_deathAnimationStopwatch.ElapsedMilliseconds > _mobAttributes.DeathAnimationDelay)
                    {
                        _animator.Play(CorpseSpriteLeft);
                        _deathAnimationStopwatch.Stop();
                    }
                }
                else
                {
                    _animator.Play(_mobAttributes.MoveSpeed > 0 ? MoveSpriteLeft : IdleSpriteLeft );
                }
            }
            else if (angle > 180 && angle <= 270)
            {
                if ( _mobAttributes.Dead )
                {
                    if ( !_deathAnimationPlayed )
                    {
                        _animator.Play(DieSpriteUp);
                        _deathAnimationStopwatch.Start();
                        _deathAnimationPlayed = true;
                    }
                    else if (_deathAnimationStopwatch.ElapsedMilliseconds > _mobAttributes.DeathAnimationDelay)
                    {
                        _animator.Play(CorpseSpriteUp);
                        _deathAnimationStopwatch.Stop();
                    }
                }
                else
                {
                    _animator.Play(_mobAttributes.MoveSpeed > 0 ? MoveSpriteUp : IdleSpriteUp );
                }
            }
            else
            {
                if (_mobAttributes.Dead)
                {
                    if ( !_deathAnimationPlayed )
                    {
                        _animator.Play(DieSpriteRight);
                        _deathAnimationStopwatch.Start();
                        _deathAnimationPlayed = true;
                    }
                    else if (_deathAnimationStopwatch.ElapsedMilliseconds > _mobAttributes.DeathAnimationDelay)
                    {
                        _animator.Play(CorpseSpriteRight);
                        _deathAnimationStopwatch.Stop();
                    }
                }
                else
                {
                    _animator.Play(_mobAttributes.MoveSpeed > 0 ? MoveSpriteRight : IdleSpriteRight);
                }
            }
            #endregion
        }

    }
}
