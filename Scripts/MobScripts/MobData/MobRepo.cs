using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal class MobRepo : MonoBehaviour
    {

        public static MobRepo Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public List<MobAttributes> MobAttributesList;
        public Dictionary<string, int> NameToIndex = new Dictionary<string, int>();
        [HideInInspector] public List<string> IndexToName;
        protected void Start()
        {
            if (MobAttributesList == null)
            {
                Debug.LogWarning("Mob Attributes List was not set!!!");
                return;
            }
            IndexToName = new List<string>(MobAttributesList.Count);
            for (var i = 0; i < MobAttributesList.Count; i++) IndexToName.Add(null);
            foreach (var mobAttribute in MobAttributesList)
            {
                NameToIndex.Add(mobAttribute.Name, mobAttribute.Index);
                IndexToName[mobAttribute.Index] = mobAttribute.Name;
            }
        }
    }
}
