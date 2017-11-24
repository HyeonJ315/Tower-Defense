using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    public class ProjectileAttributes
    {
        public int    ProjectileNumber;
        public float  Speed;
        public float  Damage;
        public float  DeathClipLength = 1.0f; // in seconds.
        public string Type;

        public ProjectileAttributes( ProjectileAttributes cpy )
        {
            if (cpy == null)
            {
                Debug.Log("Cannot copy a null constructor.");
                return;
            }
            ProjectileNumber = cpy.ProjectileNumber;
            Speed            = cpy.Speed;
            Damage           = cpy.Damage;
            DeathClipLength  = cpy.DeathClipLength;
            Type             = cpy.Type;
        }

        public ProjectileAttributes( TurretAttributes turret )
        {
            ProjectileNumber = turret.ProjectileNumber;
            Speed            = turret.ProjectileSpeed;
            Damage           = turret.AttackGround;
            Type             = turret.AttackElement;
        }

        public ProjectileAttributes() { }
    }
}
