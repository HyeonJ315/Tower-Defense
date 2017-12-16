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

        public List<MobAttributes> MobAttributesList;
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
            IndexToName = new List<string>(MobAttributesList.Count);
            for ( var i = 0; i < MobAttributesList.Count; i++) IndexToName.Add(null);
            foreach ( var mobAttribute in MobAttributesList )
            {
                NameToIndex.Add(mobAttribute.Name, mobAttribute.Index);
                IndexToName[mobAttribute.Index] = mobAttribute.Name;
            }
            MobCount = MobAttributesList.Count;
        }

        protected void Start()
        {
            MobTypeList = new List<List<MobAttributes>>();
            foreach ( var element in ElementRepository.Instance.ElementAttributesList )
            {
                MobTypeList.Add( new List<MobAttributes>() );
            }
            foreach ( var mobAttribute in MobAttributesList )
            {
                var index = ElementRepository.Instance.NameToIndex[ mobAttribute.Types[0] ];
                MobTypeList[ index ].Add( mobAttribute );
            }
        }
    }
}
