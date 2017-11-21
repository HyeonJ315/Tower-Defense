using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    internal class ProjectileRotation2D : ProjectileRotation
    {
        public string IdleSpriteLeft  = string.Empty;
        public string IdleSpriteRight = string.Empty;

        public string DieSpriteLeft   = string.Empty;
        public string DieSpriteRight  = string.Empty;

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
            if ( angle > 0 && angle <= 180 && !ProjectileAttributes.Hit && IdleSpriteLeft != string.Empty)
            {
                Animator.Play(IdleSpriteLeft);
            }
            else if (angle > 180 && angle <= 360 && !ProjectileAttributes.Hit && IdleSpriteRight != string.Empty)
            {
                Animator.Play(IdleSpriteRight);
            }
            else if (angle > 0 && angle <= 180 && ProjectileAttributes.Hit && DieSpriteLeft != string.Empty && !AlreadyHit)
            {
                Animator.Play(DieSpriteLeft);
                AlreadyHit = true;
            }
            else if (angle > 180 && angle <= 360 && ProjectileAttributes.Hit && DieSpriteRight != string.Empty && !AlreadyHit)
            {
                Animator.Play(DieSpriteRight);
                AlreadyHit = true;
            }
        }
    }
}
