using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.ElementScripts;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    internal class TurretRepository : MonoBehaviour
    {
        public static TurretRepository Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public List<TurretAttributes> TurretAttributesList;
        public GameObject ShadowTile;
        public Dictionary<string, int> NameToIndex = new Dictionary<string, int>();
        [HideInInspector] public List<string> IndexToName;
        [HideInInspector] public int TurretCount { private set; get; }
        public List<List<TurretAttributes>> TurretTypeList;
        protected void OnEnable()
        {
            if (TurretAttributesList == null)
            {
                Debug.LogWarning("Turret Attributes List was not set!!!");
                return;
            }
            IndexToName = new List<string>(TurretAttributesList.Count);
            for (var i = 0; i < TurretAttributesList.Count; i++) IndexToName.Add(null);
            for (var i = 0; i < TurretAttributesList.Count; i++ )
            {
                var turretAttribute = TurretAttributesList[i];
                NameToIndex.Add(turretAttribute.Name, turretAttribute.Index);
                IndexToName[ i ] = turretAttribute.Name;
                turretAttribute.Index = i;
            }
            TurretCount = TurretAttributesList.Count;
        }

        protected void Start()
        {
            TurretTypeList = new List<List<TurretAttributes>>();
            foreach (var unused in ElementRepository.Instance.ElementAttributesList)
                TurretTypeList.Add(new List<TurretAttributes>());
            foreach (var mobAttribute in TurretAttributesList)
            {
                var index = ElementRepository.Instance.NameToIndex[mobAttribute.Types[0]];
                TurretTypeList[index].Add(mobAttribute);
            }
        }
    }
}
