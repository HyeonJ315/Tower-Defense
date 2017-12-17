using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using Assets.Scripts.ElementScripts;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal class MobRepository : MonoBehaviour
    {

        public static MobRepository Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public List<MobAttributes> MobAttributesList = null;
        public List< MobAttributesList > MobAttributesListList = null;

        [HideInInspector]
        public List<MobAttributes> FinalMobAttributesList = new List<MobAttributes>();

        public int MobCount { private set; get; }

        public Dictionary<string, int> NameToIndex = new Dictionary<string, int>();
        [HideInInspector] public List<string> IndexToName;
        public List< List< MobAttributes > > MobTypeList;
        protected void OnEnable()
        {
            if (MobAttributesList == null)
            {
                Debug.LogWarning("Mob Attributes List was not set!!!");
                return;
            }

            foreach ( var mobAttribute in MobAttributesList )
            {
                FinalMobAttributesList.Add( mobAttribute );
                IndexToName.Add(null);
            }
            if (MobAttributesListList != null)
            {
                foreach (var mobAttributesList in MobAttributesListList)
                {   foreach ( var mobAttribute in mobAttributesList.List )
                    {
                        FinalMobAttributesList.Add( mobAttribute );
                        IndexToName.Add(null);
                    }
                }
            }

            for (var index = 0; index < FinalMobAttributesList.Count; index++)
            {
                var mobAttribute = FinalMobAttributesList[index];
                mobAttribute.Index = index;
                NameToIndex.Add( mobAttribute.Name, index );
                IndexToName[ index ] = mobAttribute.Name;
            }
            MobCount = FinalMobAttributesList.Count;
        }

        protected void Start()
        {
            MobTypeList = new List<List<MobAttributes>>();
            foreach ( var unused in ElementRepository.Instance.ElementAttributesList )
            {
                MobTypeList.Add( new List<MobAttributes>() );
            }
            foreach ( var mobAttribute in FinalMobAttributesList )
            {
                var indexType1 = ElementRepository.Instance.NameToIndex[ mobAttribute.Types[0] ];
                MobTypeList[indexType1].Add(mobAttribute);
                if ( string.IsNullOrEmpty(mobAttribute.Types[1]) ) continue;
                var indexType2 = ElementRepository.Instance.NameToIndex[mobAttribute.Types[1]];
                MobTypeList[indexType2].Add(mobAttribute);
            }
        }
    }
}
