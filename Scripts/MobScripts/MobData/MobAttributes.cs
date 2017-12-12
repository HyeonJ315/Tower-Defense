using System;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    [Serializable]
    public class MobAttributes
    {
        public string Name;
        public int    Index;
        public GameObject Prefab;
        public GameObject Icon;
        public float  Health;
        public float  Defense;
        public float  SpecialDefense;
        public float  MoveSpeed;
        public bool   Flying;
        public float  HitSphere;
        public int    DeathAnimationDelay;
        public int    DeathFadeDelay;
        public int    DeathFadeDuration;
        public float  HealthCanvasY;
        public string PrevEvolution;
        public string NextEvolution;
        public string[] Types = new string[2];

        public MobAttributes()
        {
            Types[0] = null;
            Types[1] = null;
        }

        public MobAttributes(MobAttributes cpy)
        {
            if (cpy == null)
            {
                Debug.Log("Cannot copy a null constructor.");
                return;
            }
            Name                = cpy.Name;
            Index               = cpy.Index;
            Health              = cpy.Health;
            Defense             = cpy.Defense;
            SpecialDefense      = cpy.SpecialDefense;
            MoveSpeed           = cpy.MoveSpeed;
            Flying              = cpy.Flying;
            HitSphere           = cpy.HitSphere;
            DeathAnimationDelay = cpy.DeathAnimationDelay;
            DeathFadeDelay      = cpy.DeathFadeDelay;
            DeathFadeDuration   = cpy.DeathFadeDuration;
            HealthCanvasY       = cpy.HealthCanvasY;
            Types[0] = cpy.Types[0];
            Types[1] = cpy.Types[1];
        }
    }
}
