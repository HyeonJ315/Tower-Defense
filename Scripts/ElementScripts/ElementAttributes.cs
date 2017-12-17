using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.ElementScripts
{
    [Serializable]
    internal class ElementAttributes
    {
        public string Name = string.Empty;
        [HideInInspector] public int Index;
        public GameObject Icon = null;
        public List< DamageModifier > DamageModifiers = null;

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
        public string Name = string.Empty;
        [HideInInspector] public int Index = -1;
        public float Modifier = 0;
    }
}
