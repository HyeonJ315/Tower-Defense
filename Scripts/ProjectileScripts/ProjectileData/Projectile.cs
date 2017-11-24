using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.MobScripts.MobManagement;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    public class Projectile : MonoBehaviour
    {
        public ProjectileAttributes ProjectileAttributes;
        private MobModifierRpc       _mobModifierRpc;

        private Vector3 _destination;
        private float _hitSphere;
        private bool _destinationSet;

        public bool ServerProjectile;
        public bool Hit;
        public Mob Target;

        private GameObject _target;

        // Use this for initialization
        protected void Start ()
        {
            _destination = transform.position;
        }

        protected void FixedUpdate()
        {   
            _handleMovement();
            _handleCollision();
        }

        private void _handleMovement()
        {
            #region Is the projectile hit or is the target dead?

            if ( Hit )
            {
                return;
            }

            #endregion

            #region Find the target.

            if (ProjectileAttributes == null)
            {
                return;
            }

            if (Target.MobHash == 0)
            {
                return;
            }

            if ( _target == null )
            {
                if (MobTrackerDictionary.Instance.TryGetValue(Target.MobHash, out _target))
                {
                    Target = _target.GetComponent<Mob>();
                    _hitSphere = Target.MobAttributesCurrent.HitSphere;
                }
                else
                {
                    if ( !_destinationSet )
                    {
                        Destroy( gameObject );
                        return;
                    }
                }
            }

            #endregion

            #region Move to the target

            if( _target != null )_destination = Target.transform.position;
            _destinationSet = true;

            var moveDir = Vector3.Normalize( _destination - transform.position );
            var displacement = moveDir * ProjectileAttributes.Speed * Time.fixedDeltaTime;
            transform.position += displacement;
            transform.LookAt( transform.position + moveDir );

            #endregion
        }

        private void _handleCollision()
        {
            if ( Hit ) return;

            if ( _mobModifierRpc == null )
            {
                _mobModifierRpc = MobModifierRpcServer.Instance;
                return;
            }

            if ( _hitSphere <= Vector3.Distance( _destination, transform.position) ) return;

            if ( ServerProjectile && _target != null && !Target.Dead )
            {
                Target.MobAttributesCurrent.Health -= ProjectileAttributes.Damage;
                _mobModifierRpc.UpdateHealthSendRpc( Target.MobHash, Target.MobAttributesCurrent.Health );
            }
            Hit = true;
            Destroy( gameObject, ProjectileAttributes.DeathClipLength );
        }
    }
}
