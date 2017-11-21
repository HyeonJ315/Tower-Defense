using Assets.Scripts.MobScripts.MobData;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    class ProjectileAttributes : MonoBehaviour
    {
        public uint TargetNumber = 0;
        public MobAttributesMono Target = null;
        public float Speed;
        public float Damage;
        public bool  Hit = false;
        public float DeathClipLength = 0.5f;
        public bool ServerProjectile = false;
    }
}
