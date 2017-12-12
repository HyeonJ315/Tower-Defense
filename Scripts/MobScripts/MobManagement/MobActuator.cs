﻿using System;
using Assets.Scripts.MobScripts.MobData;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobManagement
{
    internal class MobActuator : MonoBehaviour
    {
        #region Singleton

        public static MobActuator Instance { get; private set; }
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

            string mobName;
            if (!MobRepository.Instance.MobIdToName.TryGetValue(mobNumber, out mobName))
                return false;

            if ( _mobsHierarchyGameObject == null )
                _mobsHierarchyGameObject = new GameObject { name = "Mobs" };

            mobGameObject =
                Instantiate( Resources.Load( MobRepository.MobDir + "/" + mobNumber + "_" + mobName + "/" + mobName ) ) as GameObject;
            if (mobGameObject == null) return false;
            mobGameObject.transform.position = position;

            mobGameObject.transform.SetParent( _mobsHierarchyGameObject.transform );

            #endregion

            #region Set the Mob's attributes

            var mob = mobGameObject.GetComponent<Mob>();

            mob.transform.position = position;
            mob.Initialize( playerNumber, mobNumber, mobName, teamGroup, pathName  );

            #endregion

            return true;
        }
    }
}