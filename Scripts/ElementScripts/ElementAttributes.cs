using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ElementScripts
{
    [Serializable]
    internal class ElementAttributes
    {
        public string Name;
        [HideInInspector] public int    Index;
        public GameObject Icon;
        public List< DamageModifier > DamageModifiers;

        private bool _initialized;
        public bool Initialize()
        {
            if (_initialized)
                return false;
            foreach ( var modifier in DamageModifiers )
                modifier.Index = ElementRepository.Instance.NameToIndex[ Name ];
            return _initialized = true;
        }
    }

    [Serializable]  
    internal class DamageModifier
    {
        public string Name;
        [HideInInspector] public int Index = -1;
        public float  Modifier;
    }
}
