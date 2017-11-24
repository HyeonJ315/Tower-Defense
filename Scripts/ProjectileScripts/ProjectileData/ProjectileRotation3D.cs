
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    internal class ProjectileRotation3D : ProjectileRotation
    {
        protected string IdleSpriteFront = "IdleFront";
        protected string IdleSpriteLeft  = "IdleLeft" ;
        protected string IdleSpriteUp    = "IdleUp"   ;
        protected string IdleSpriteRight = "IdleRight";

        protected string DieSpriteFront  = "HitFront" ;
        protected string DieSpriteLeft   = "HitLeft"  ;
        protected string DieSpriteUp     = "HitUp"    ;
        protected string DieSpriteRight  = "HitRight" ;

        protected override float CalculateAngle(Vector3 v1, Vector3 v2)
        {
            v1.y = v2.y = 0;
            var angle = Vector3.Angle(v1, v2);
            if (Vector3.Cross(v1, v2).y < 0) angle = 360.0f - angle;
            angle -= 135;
            angle = angle < 0 ? angle + 360 : angle;
            return angle;
        }

        protected override void PlayAnimation(float angle)
        {
            if (angle > 0 && angle <= 90)
            {
                if (!Projectile.Hit)
                    Animator.Play(IdleSpriteFront);
                else if (!AlreadyHit)
                {
                    Animator.Play(DieSpriteFront);
                    AlreadyHit = true;
                }
            }
            else if (angle > 90 && angle <= 180)
            {
                if (!Projectile.Hit)
                    Animator.Play(IdleSpriteLeft);
                else if (!AlreadyHit)
                {
                    Animator.Play(DieSpriteLeft);
                    AlreadyHit = true;
                }
            }
            else if (angle > 180 && angle <= 270)
            {
                if (!Projectile.Hit)
                    Animator.Play(IdleSpriteUp);
                else if (!AlreadyHit)
                {
                    Animator.Play(DieSpriteUp);
                    AlreadyHit = true;
                }
            }
            else
            {
                if (!Projectile.Hit)
                    Animator.Play(IdleSpriteRight);
                else if (!AlreadyHit)
                {
                    Animator.Play(DieSpriteRight);
                    AlreadyHit = true;
                }
            }
        }
    }
}
