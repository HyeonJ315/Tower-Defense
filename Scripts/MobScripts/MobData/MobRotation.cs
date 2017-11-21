using System.Diagnostics;
using Assets.Scripts.RTSCamera;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal abstract class MobRotation : MonoBehaviour
    {
        public string CameraName = "RtsCamera";
        protected Animator     Animator;
        protected GameObject    Camera;
        protected Transform     ParentTransform;
        protected Material      SpriteMaterial;
        protected MobAttributesMono MobAttributesMono;

        protected Stopwatch DeathAnimationStopwatch;
        protected Stopwatch DeadStopwatch;

        protected bool DeathAnimationPlayed;

        protected void Start()
        {
            Animator = transform.parent.gameObject.GetComponentInChildren<Animator>();
            MobAttributesMono = transform.parent.gameObject.GetComponents<MobAttributesMono>()[1];
            ParentTransform = transform.parent.transform;
            DeathAnimationStopwatch = new Stopwatch();
            DeadStopwatch           = new Stopwatch();
            SpriteMaterial          = transform.parent.GetComponentInChildren<Animator>()
                                          .transform.gameObject.GetComponent<SpriteRenderer>().material;
            DeathAnimationStopwatch.Stop();
            DeathAnimationStopwatch.Reset();
            DeadStopwatch.Stop();
            DeadStopwatch.Reset();
            RTS_Camera.CameraDictionary.TryGetValue( CameraName, out Camera );
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

            var angle = CalculateAngle( v1, v2 );
            PlayAnimation( angle );

            #endregion

            #region Death handler.

            HandleDeath();

            #endregion
        }

        protected abstract float CalculateAngle(Vector3 v1, Vector3 v2);
        protected abstract void PlayAnimation( float angle );

        protected virtual void HandleDeath()
        {
            if (MobAttributesMono.Dead)
            {
                if (!DeadStopwatch.IsRunning)
                {
                    DeadStopwatch.Start();
                }
                if (DeadStopwatch.ElapsedMilliseconds < MobAttributesMono.DeathFadeDelay) return;
                var fadeValue = (float)(DeadStopwatch.ElapsedMilliseconds - MobAttributesMono.DeathFadeDuration) / MobAttributesMono.DeathFadeDuration;
                var spriteColor = SpriteMaterial.color;
                if (fadeValue < 1.0f)
                {
                    spriteColor.a = 1.0f - fadeValue;
                    SpriteMaterial.color = spriteColor;
                }
                else
                {
                    Destroy(ParentTransform.gameObject);
                }
            }

            if (MobAttributesMono.Health <= 0)
            {
                MobAttributesMono.Dead = true;
            }
        }
    }
}
