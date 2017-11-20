using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.MobScripts.MobManagement;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    public class Projectile : MonoBehaviour
    {
        private ProjectileAttributes _projectileAttributes;
        private MobModifierRpc _mobModifierRpc;

        private Vector3 _destination;
        private Vector3 _hitSphere;
        // Use this for initialization
        protected void Start ()
        {
            _projectileAttributes = GetComponent<ProjectileAttributes>();
            _destination          = transform.position;
        }

        protected void FixedUpdate()
        {   
            _handleMovement();
            _handleCollision();
        }

        private void _handleMovement()
        {
            #region Find the target.

            if (_projectileAttributes == null)
            {
                return;
            }

            if (_projectileAttributes.TargetNumber == 0)
            {
                return;
            }

            if ( _projectileAttributes.Target == null )
            {
                GameObject target;
                if ( !MobTrackerDictionary.Instance.TryGetValue( _projectileAttributes.TargetNumber, out target) )
                {
                    return;
                }
                _projectileAttributes.Target       = target.GetComponents<MobAttributes>()[1];
                _projectileAttributes.TargetNumber = target.GetComponent<Mob>().MobNumber;
            }

            #endregion

            #region Is the projectile hit or is the target dead?

            if ( _projectileAttributes.Hit )
            {
                return;
            }

            #endregion

            #region Move to the target

            _destination = _projectileAttributes.Target.transform.position;
            var moveDir = Vector3.Normalize( _destination - transform.position );
            var displacement = moveDir * _projectileAttributes.Speed * Time.fixedDeltaTime;
            transform.position += displacement;
            transform.LookAt( transform.position + moveDir );

            #endregion
        }

        private void _handleCollision()
        {
            if ( _projectileAttributes.Hit ) return;

            if ( _mobModifierRpc == null )
            {
                _mobModifierRpc = MobModifierRpcServer.Instance;
                return;
            }

            if ( _projectileAttributes.Target.HitSphere <= 
                 Vector3.Distance( _destination, transform.position) ) return;

            if ( _projectileAttributes.ServerProjectile && !_projectileAttributes.Target.Dead )
            {
                _projectileAttributes.Target.Health -= _projectileAttributes.Damage;
                _mobModifierRpc.UpdateHealthSendRpc( _projectileAttributes.TargetNumber, _projectileAttributes.Target.Health );
            }
            _projectileAttributes.Hit = true;
            Destroy( gameObject, _projectileAttributes.DeathClipLength );
        }
    }
}
