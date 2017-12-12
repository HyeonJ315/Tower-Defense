using System.Diagnostics;
using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.NetworkManagement;
using Assets.Scripts.ProjectileScripts.ProjectileData;
using Assets.Scripts.ProjectileScripts.ProjectileManagement;
using Assets.Scripts.TrackingDictionaries;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    public class Turret : MonoBehaviour
    {
        public int    TurretNumber { get; private set; }
        public string TurretName   { get; private set; }
        public int    TeamGroup    { get; private set; }
        public int    PlayerNumber { get; private set; }
        private TurretAttributes _turretAttributesReference;
        public  TurretAttributes  TurretAttributes;
        private TurretRotation   _turretRotation;
        private readonly Stopwatch _timeSinceLastAttack = new Stopwatch();

        public bool Attacking      { get; private set; }
        private Mob _attackingMob;

        private bool _initialized;

        public void Initialize( int playerNumber, int turretNumber, string turretName, int teamGroup )
        {
            if ( _initialized )
            {
                UnityEngine.Debug.Log( "" + turretNumber + "_" + turretName + " already initialized." );
                return;
            }
            if ( !TurretRepository.Instance.TurretFullNameToAttributes.TryGetValue( "" + turretNumber + "_" + turretName,
                out _turretAttributesReference) )
            {
                UnityEngine.Debug.Log( " Can't find " + turretNumber + "_" + turretName + " in the Dictionary." );
                return;
            }
            TurretAttributes = new TurretAttributes( _turretAttributesReference );
            PlayerNumber = playerNumber;
            TurretNumber = turretNumber;
            TurretName   = turretName;
            TeamGroup    = teamGroup;
            _timeSinceLastAttack.Stop();
            _timeSinceLastAttack.Reset();
            _turretRotation = GetComponentInChildren<TurretRotation>();
            TurretListTrackerDictionary.Instance.InsertEntry( playerNumber, gameObject );
            //UnityEngine.Debug.Log( "Created turret for player " + playerNumber + ": " + turretNumber + "_" + turretName );
            _initialized = true;
        }

        protected void OnDestroy()
        {
            TurretListTrackerDictionary.Instance.DeleteEntry( PlayerNumber, gameObject );
        }

        public void AlertOfMobPresence( GameObject mobGameObject )
        {
            var thisMob = mobGameObject.GetComponent<Mob>();
            if ( _attackingMob == null && thisMob.TeamGroup != TeamGroup )
            {
                _attackingMob = thisMob;
            }
        }

        protected void FixedUpdate()
        {
            if (!_initialized)
            {
                UnityEngine.Debug.Log( "Turret Not Initialized." );
                return;
            }
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
            if ( _attackingMob.gameObject.GetComponent<Mob>().Dead )
            {
                _attackingMob = null;
                Attacking = false;
                return;
            }

            // is the mob in range to attack?
            if ( Vector3.Distance( transform.position, _attackingMob.transform.position ) <= TurretAttributes.Range )
            {
                _turretRotation.gameObject.transform.LookAt( _attackingMob.transform.position );
                Attacking = true;

                // is the turret gameobject part of the server?
                if ( NetworkingManager.Instance.IsClient ) return;

                // does the turret have access to the projectile manager remote procedure calls?
                if ( ProjectileManagerRpcServer.Instance == null ) return;

                if ( !_timeSinceLastAttack.IsRunning )
                {
                    GameObject tmp;
                    ProjectileActuator.Instance.ProjectileSpawn( TurretAttributes, transform.position, _attackingMob.MobHash, out tmp );
                    var projectile = tmp.GetComponent<Projectile>();
                    projectile.ServerProjectile = true;

                    ProjectileManagerRpcServer.Instance.ProjectileSpawnSendRpc( transform.position + TurretAttributes.ProjectileSpawnOffset, 
                                                                                TurretAttributes.ProjectileNumber, _attackingMob.MobHash );
                    _timeSinceLastAttack.Start();
                }
                else if  ( _timeSinceLastAttack.ElapsedMilliseconds >= TurretAttributes.AttackSpeed )
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
}
