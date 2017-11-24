using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal class MobRotation2D : MobRotation
    {
        protected string IdleSpriteLeft    = "IdleLeft"   ;
        protected string IdleSpriteRight   = "IdleRight"  ;

        protected string MoveSpriteLeft    = "WalkLeft"   ;
        protected string MoveSpriteRight   = "WalkRight"  ;

        protected string DieSpriteLeft     = "DieLeft"    ;
        protected string DieSpriteRight    = "DieRight"   ;

        protected string CorpseSpriteLeft  = "CorpseLeft" ;
        protected string CorpseSpriteRight = "CorpseRight";


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
                if ( MobCurrent.Dead )
                {
                    if (!DeathAnimationPlayed)
                    {
                        Animator.Play(DieSpriteLeft);
                        DeathAnimationStopwatch.Start();
                        DeathAnimationPlayed = true;
                    }
                    else if (DeathAnimationStopwatch.ElapsedMilliseconds > MobAttributesCurrent.DeathAnimationDelay)
                    {
                        Animator.Play(CorpseSpriteLeft);
                        DeathAnimationStopwatch.Stop();
                    }
                }
                else
                {
                    Animator.Play(MobAttributesCurrent.MoveSpeed > 0 ? MoveSpriteLeft : IdleSpriteLeft);
                }
            }
            else
            {
                if ( MobCurrent.Dead )
                {
                    if (!DeathAnimationPlayed)
                    {
                        Animator.Play(DieSpriteRight);
                        DeathAnimationStopwatch.Start();
                        DeathAnimationPlayed = true;
                    }
                    else if (DeathAnimationStopwatch.ElapsedMilliseconds > MobAttributesCurrent.DeathAnimationDelay)
                    {
                        Animator.Play(CorpseSpriteRight);
                        DeathAnimationStopwatch.Stop();
                    }
                }
                else
                {
                    Animator.Play(MobAttributesCurrent.MoveSpeed > 0 ? MoveSpriteRight : IdleSpriteRight);
                }
            }
        }
    }
}
