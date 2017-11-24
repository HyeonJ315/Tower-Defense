using Assets.Scripts.MobScripts.MobData;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.MobScripts.MobManagement
{
    public abstract class MobModifierRpc : NetworkBehaviour
    {
        public abstract void UpdateMoveStateSendRpc( uint mobHashNumber, Vector3 position, Vector3 destination );
        public abstract void UpdateHealthSendRpc( uint mobHashNumber, float health );

        protected bool UpdateMoveState( uint mobHashNumber, Vector3 position, Vector3 destination )
        {
            GameObject mob;
            if ( !MobTrackerDictionary.Instance.TryGetValue(mobHashNumber, out mob) ) return false;
            mob.GetComponent<Mob>().Destination = destination;
            if (Vector3.Distance(mob.transform.position, position) > 2.0f) mob.transform.position = position;
            return true;
        }

        protected bool UpdateHealth( uint mobHashNumber, float health )
        {
            GameObject mob;
            if ( !MobTrackerDictionary.Instance.TryGetValue(mobHashNumber, out mob) ) return false;
            mob.GetComponent<Mob>().MobAttributesCurrent.Health = health;
            return true;
        }

    }
}
