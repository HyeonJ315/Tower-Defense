using System;
using Assets.Scripts.MobScripts.MobData;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobManagement
{
    internal class MobManager : MonoBehaviour
    {
        #region Singleton

        public static MobManager Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        private GameObject _mobsHierarchyGameObject;

        public bool MobSpawn( int mobNumber, int playerNumber, int teamGroup, Vector3 position, string pathName, out GameObject mobGameObject )
        {
            mobGameObject = null;
            #region Create the Mob

            if ( !Enum.IsDefined( typeof(MobPrefabs), mobNumber) )
                return false;
            var mobName = Enum.GetName( typeof(MobPrefabs), mobNumber );

            if ( _mobsHierarchyGameObject == null )
            {
                _mobsHierarchyGameObject = new GameObject { name = "Mobs" };
            }

            mobGameObject =
                Instantiate(Resources.Load( MobPrefabDictionary.MobPrefabsDirectory + "/" + mobName ) ) as GameObject;
            if (mobGameObject == null) return false;
            mobGameObject.transform.position = position;

            mobGameObject.transform.SetParent( _mobsHierarchyGameObject.transform );

            #endregion

            #region Set the Mob's attributes

            var mob = mobGameObject.GetComponent<Mob>();

            mob.PathName           = pathName;
            mob.transform.position = position;
            mob.PlayerNumber       = playerNumber;
            mob.Team               = teamGroup;

            #endregion

            return true;
        }
    }
}
