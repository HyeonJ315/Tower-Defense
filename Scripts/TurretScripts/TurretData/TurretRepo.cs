using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    internal class TurretRepo : MonoBehaviour
    {
        public static TurretRepo Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public List<TurretAttributes> TurretAttributesList;
        public Dictionary<string, int> NameToIndex = new Dictionary<string, int>();
        [HideInInspector] public List<string> IndexToName;
        protected void Start()
        {
            if (TurretAttributesList == null)
            {
                Debug.LogWarning("Turret Attributes List was not set!!!");
                return;
            }
            IndexToName = new List<string>(TurretAttributesList.Count);
            for (var i = 0; i < TurretAttributesList.Count; i++) IndexToName.Add(null);
            foreach (var turretAttribute in TurretAttributesList)
            {
                NameToIndex.Add(turretAttribute.Name, turretAttribute.Index);
                IndexToName[turretAttribute.Index] = turretAttribute.Name;
            }
        }
    }
}
