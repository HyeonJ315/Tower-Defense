﻿using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    internal class TurretRotation2D : TurretRotation
    {
        public string AttackSpriteLeft  = string.Empty;
        public string AttackSpriteRight = string.Empty;

        public string IdleSpriteLeft  = string.Empty;
        public string IdleSpriteRight = string.Empty;

        protected override float CalculateAngle( Vector3 v1, Vector3 v2 )
        {
            v1.y = v2.y = 0;
            var angle = Vector3.Angle(v1, v2);
            if (Vector3.Cross(v1, v2).y < 0) angle = 360.0f - angle;
            angle -= 180;
            angle = angle < 0 ? angle + 360 : angle;
            return angle;
        }

        protected override void PlayAnimation( float angle )
        {
            if (angle > 0 && angle <= 180)
            {
                Animator.Play(Turret.Attacking ? AttackSpriteLeft : IdleSpriteLeft);
            }
            else
            {
                Animator.Play(Turret.Attacking ? AttackSpriteRight : IdleSpriteRight);
            }
        }
    }
}
