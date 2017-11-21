using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal class MobRotation2D : MobRotation
    {
        public string IdleSpriteLeft    = string.Empty;
        public string IdleSpriteRight   = string.Empty;

        public string MoveSpriteLeft    = string.Empty;
        public string MoveSpriteRight   = string.Empty;

        public string DieSpriteLeft     = string.Empty;
        public string DieSpriteRight    = string.Empty;

        public string CorpseSpriteLeft  = string.Empty;
        public string CorpseSpriteRight = string.Empty;

        protected override float CalculateAngle(Vector3 v1, Vector3 v2)
        {
            v1.y = v2.y = 0;
            var angle = Vector3.Angle(v1, v2);
            if (Vector3.Cross(v1, v2).y < 0) angle = 360.0f - angle;
            angle -= 180;
            angle = angle < 0 ? angle + 360 : angle;
            return angle;
        }

        protected override void PlayAnimation(float angle)
        {
            if (angle > 0 && angle <= 180)
            {
                if (MobAttributesMono.Dead)
                {
                    if (!DeathAnimationPlayed)
                    {
                        Animator.Play(DieSpriteLeft);
                        DeathAnimationStopwatch.Start();
                        DeathAnimationPlayed = true;
                    }
                    else if (DeathAnimationStopwatch.ElapsedMilliseconds > MobAttributesMono.DeathAnimationDelay)
                    {
                        Animator.Play(CorpseSpriteLeft);
                        DeathAnimationStopwatch.Stop();
                    }
                }
                else
                {
                    Animator.Play(MobAttributesMono.MoveSpeed > 0 ? MoveSpriteLeft : IdleSpriteLeft);
                }
            }
            else
            {
                if (MobAttributesMono.Dead)
                {
                    if (!DeathAnimationPlayed)
                    {
                        Animator.Play(DieSpriteRight);
                        DeathAnimationStopwatch.Start();
                        DeathAnimationPlayed = true;
                    }
                    else if (DeathAnimationStopwatch.ElapsedMilliseconds > MobAttributesMono.DeathAnimationDelay)
                    {
                        Animator.Play(CorpseSpriteRight);
                        DeathAnimationStopwatch.Stop();
                    }
                }
                else
                {
                    Animator.Play(MobAttributesMono.MoveSpeed > 0 ? MoveSpriteRight : IdleSpriteRight);
                }
            }
        }
    }
}
