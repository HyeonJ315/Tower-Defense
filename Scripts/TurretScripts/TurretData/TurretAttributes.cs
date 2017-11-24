﻿using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    public class TurretAttributes
    {
        public int     ProjectileNumber;
        public float   AttackGround;
        public float   AttackAir;
        public int     AttackSpeed;
        public string  AttackElement;
        public float   ProjectileSpeed;
        public float   Splash;
        public float   Range;
        public uint    Cost;
        public Vector3 ProjectileSpawnOffset;

        public string[] Types = new string[2];

        public TurretAttributes( TurretAttributes cpy )
        {
            if ( cpy == null )
            {
                Debug.Log( "Cannot copy a null constructor." );
                return;
            }

            ProjectileNumber      = cpy.ProjectileNumber;
            AttackGround          = cpy.AttackGround;
            AttackAir             = cpy.AttackAir;
            AttackSpeed           = cpy.AttackSpeed;
            AttackElement         = cpy.AttackElement;
            ProjectileSpeed       = cpy.ProjectileSpeed;
            Splash                = cpy.Splash;
            Range                 = cpy.Range;
            Cost                  = cpy.Cost;
            ProjectileSpawnOffset = cpy.ProjectileSpawnOffset;
            Types[0] = cpy.Types[0];
            Types[1] = cpy.Types[1];
        }

        public TurretAttributes() { }
    }
}
