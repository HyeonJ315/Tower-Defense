using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal class MobAttributesMono : MonoBehaviour
    {
        public float Health;
        public float Defense;
        public float SpecialDefense;
        public float MoveSpeed;
        public bool  Flying;
        public float HitSphere;
        public bool  Dead = false;
        public int   DeathAnimationDelay;
        public int   DeathFadeDelay;
        public int   DeathFadeDuration;
        public string[] Types = new string[2];
    }

    internal class MobAttributes
    {
        public float Health;
        public float Defense;
        public float SpecialDefense;
        public float MoveSpeed;
        public bool Flying;
        public float HitSphere;
        public bool Dead = false;
        public int DeathAnimationDelay;
        public int DeathFadeDelay;
        public int DeathFadeDuration;
        public string[] Types = new string[2];

        public MobAttributes()
        {
            Types[0] = null;
            Types[1] = null;
        }
    }
}
