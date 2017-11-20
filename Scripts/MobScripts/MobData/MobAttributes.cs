using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    class MobAttributes : MonoBehaviour
    {   public float Health;
        public float MoveSpeed;
        public bool  Flying;
        public float HitSphere;
        public float SnapThreshold;
        public bool  Dead = false;
        public int   DeathAnimationDelay;
        public int   DeathFadeDelay;
        public int   DeathFadeDuration;
    }
}
