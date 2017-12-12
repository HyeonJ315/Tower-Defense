using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ElementScripts
{
    internal class ElementRepo : MonoBehaviour
    {

        public static ElementRepo Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public List< ElementAttributes > ElementAttributesList;
        public Dictionary< string, int > NameToIndex = new Dictionary< string, int >();
        [HideInInspector] public List< string > IndexToName;
        protected void Start()
        {
            if (ElementAttributesList == null)
            {
                Debug.LogWarning( "Element Attributes List was not set!!!" );
                return;
            }
            IndexToName = new List< string >( ElementAttributesList.Count );
            for( var i = 0; i < ElementAttributesList.Count; i++ ) IndexToName.Add( null );
            foreach (var elementAttribute in ElementAttributesList)
            {
                NameToIndex.Add( elementAttribute.Name, elementAttribute.Index );
                IndexToName[elementAttribute.Index] = elementAttribute.Name;
                elementAttribute.Initialize();
            }
        }
    }
}
