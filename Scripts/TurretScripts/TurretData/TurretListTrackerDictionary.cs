using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TrackingDictionaries
{
    // Mobs place their game objects as an index number and projectiles are able to find the object through index.
    internal class TurretListTrackerDictionary
    {
        #region Singleton

        private static readonly TurretListTrackerDictionary I = new TurretListTrackerDictionary();
        static TurretListTrackerDictionary() { }
        private TurretListTrackerDictionary() { }
        public static TurretListTrackerDictionary Instance { get { return I; } }

        #endregion

        private readonly Dictionary<int, LinkedList<GameObject> > _dictionary = new Dictionary<int, LinkedList<GameObject> >();

        public virtual bool InsertEntry( int index, GameObject gameObject )
        {
           LinkedList<GameObject> goList;
            if (!_dictionary.TryGetValue(index, out goList))
            {
                var linkedList = new LinkedList<GameObject>();
                linkedList.AddLast( gameObject );
                _dictionary.Add( index, linkedList );
                return true;
            }
            goList.AddLast( gameObject );
            return true;
        }

        public virtual bool DeleteEntry( int index, GameObject gameObject )
        {
            LinkedList<GameObject> goList;
            return _dictionary.TryGetValue( index, out goList ) && goList.Remove( gameObject );
        }

        public virtual bool GetEntry( int index, out LinkedList<GameObject> goList )
        {
            return _dictionary.TryGetValue( index, out goList );
        }
    }
}
