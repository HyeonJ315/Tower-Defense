using System;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    [Serializable]
    public class TurretAttributes
    {
        public string Name;
        [HideInInspector] public int Index;
        public string PrevEvolution;
        public string NextEvolution;
        public GameObject Prefab;
        public GameObject Icon;
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

            Name                  = cpy.Name;
            Index                 = cpy.Index;
            PrevEvolution         = cpy.PrevEvolution;
            NextEvolution         = cpy.NextEvolution;
            Prefab                = cpy.Prefab;
            Icon                  = cpy.Icon;
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
