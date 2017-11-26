using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TrackingDictionaries
{
    // Mobs place their game objects as an index number and projectiles are able to find the object through index.
    internal class MobTrackerDictionary
    {
        #region Singleton

        private static readonly MobTrackerDictionary I = new MobTrackerDictionary();
        static MobTrackerDictionary() { }
        private MobTrackerDictionary() { }
        public static MobTrackerDictionary Instance { get { return I; } }

        #endregion

        private readonly Dictionary<uint, GameObject> _dictionary = new Dictionary<uint, GameObject>();

        public virtual bool InsertEntry(uint index, GameObject gameObject)
        {
            GameObject go;
            if (_dictionary.TryGetValue(index, out go))
                return false;
            _dictionary.Add(index, gameObject);
            return true;
        }

        public virtual bool DeleteEntry(uint index)
        {
            GameObject go;
            if (!_dictionary.TryGetValue(index, out go))
                return false;
            _dictionary.Remove(index);
            return true;
        }

        public virtual bool GetEntry(uint index, out GameObject go)
        {
            return _dictionary.TryGetValue(index, out go);
        }
    }
}
