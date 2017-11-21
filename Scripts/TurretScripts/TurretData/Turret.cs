using System.Diagnostics;
using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.NetworkManagement;
using Assets.Scripts.ProjectileScripts.ProjectileData;
using Assets.Scripts.ProjectileScripts.ProjectileManagement;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    public class Turret : MonoBehaviour
    {
        public int ProjectileNumber;
        public float AttackGround;
        public float AttackAir;
        public int AttackSpeed;
        public float ProjectileSpeed;
        public float Splash;
        public float Range;
        public uint  Cost;

        public bool Attacking;
        public int PlayerNumber;
        public Vector3 ProjectileSpawnOffset;

        private TurretRotation _turretRotation;
        private Animator       _animator;
        private readonly Stopwatch _timeSinceLastAttack = new Stopwatch();

        private Mob _attackingMob = null;

        public void AlertOfMobPresence( GameObject mobGameObject )
        {
            if ( _attackingMob == null )
            {
                _attackingMob = mobGameObject.GetComponent<Mob>();
            }
        }

        public void Start()
        {
            _turretRotation = GetComponentInChildren<TurretRotation>();
            _animator       = GetComponentInChildren<Animator>();
            _timeSinceLastAttack.Stop();
            _timeSinceLastAttack.Reset();
        }
        public void FixedUpdate()
        {
            _handleAttackingTheMob();
        }


        private void _handleAttackingTheMob()
        {
            // is there a mob to attack?
            if ( _attackingMob == null )
            {
                Attacking = false;
                return;
            }

            // is the mob alive?
            if ( _attackingMob.gameObject.GetComponents<MobAttributesMono>()[1].Dead )
            {
                _attackingMob = null;
                Attacking = false;
                return;
            }

            // is the mob in range to attack?
            if ( Vector3.Distance( transform.position, _attackingMob.transform.position ) <= Range )
            {
                _turretRotation.gameObject.transform.LookAt( _attackingMob.transform.position );
                Attacking = true;

                // is the turret gameobject part of the server?
                if ( NetworkingManager.Instance.IsClient ) return;

                // does the turret have access to the projectile manager remote procedure calls?
                if (ProjectileManagerRpcServer.Instance == null) return;

                if (!_timeSinceLastAttack.IsRunning)
                {
                    GameObject tmp;
                    ProjectileManager.Instance.ProjectileSpawn( ProjectileNumber, transform.position + ProjectileSpawnOffset, _attackingMob.MobNumber, out tmp );
                    tmp.GetComponent<ProjectileAttributes>().ServerProjectile = true;
                    ProjectileManagerRpcServer.Instance.ProjectileSpawnSendRpc( transform.position + ProjectileSpawnOffset, ProjectileNumber, _attackingMob.MobNumber );
                    _timeSinceLastAttack.Start();
                }
                else if  ( _timeSinceLastAttack.ElapsedMilliseconds >= AttackSpeed )
                {
                    _timeSinceLastAttack.Stop();
                    _timeSinceLastAttack.Reset();
                }
            }
            else
            {
                _attackingMob = null;
                Attacking = false;
            }
        }
    }

    public class TurretAttributes
    {
        public int ProjectileNumber;
        public float AttackGround;
        public float AttackAir;
        public int AttackSpeed;
        public float ProjectileSpeed;
        public float Splash;
        public float Range;
        public uint Cost;
        public Vector3 ProjectileSpawnOffset;
        public string[] Types = new string[2];
    }
}
