using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal class MobState : MonoBehaviour
    {
        public string CameraName = "RtsCamera";
        private Animator      _animator;
        private List< MobAttributes > _mobAttributes;
        private Stopwatch _deadStopwatch;
        private Material _spriteMaterial;

        protected void Start()
        {
            _animator      = transform.parent.gameObject.GetComponentInChildren<Animator>();
            _mobAttributes = new List< MobAttributes >( transform.parent.gameObject.GetComponents< MobAttributes >() );
            _deadStopwatch = new Stopwatch();
            _spriteMaterial  = transform.parent.GetComponentInChildren<Animator>()
                                   .transform.gameObject.GetComponent<SpriteRenderer>().material;
        }

        protected void FixedUpdate()
        {
            #region Death handler.

            if ( _mobAttributes[1].Dead )
            {
                if (!_deadStopwatch.IsRunning)
                {
                    _deadStopwatch.Start();
                }
                if (_deadStopwatch.ElapsedMilliseconds < _mobAttributes[1].DeathFadeDelay) return;
                var fadeValue = (float) ( _deadStopwatch.ElapsedMilliseconds - _mobAttributes[1].DeathFadeDuration ) / _mobAttributes[1].DeathFadeDuration;
                var spriteColor = _spriteMaterial.color;
                if (fadeValue < 1.0f)
                {
                    spriteColor.a = 1.0f - fadeValue;
                    _spriteMaterial.color = spriteColor;
                }
                else
                {
                    Destroy( gameObject.transform.parent.gameObject );
                }
            }

            #endregion

            #region Change state of the mechanim to show walking and death and idle.

            if ( _mobAttributes[1].Health <= 0 )
            {
                _mobAttributes[1].Dead = true;
            }

            #endregion
        }

    }
}