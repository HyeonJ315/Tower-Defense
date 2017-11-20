using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Debugger;
using Assets.Scripts.Shared.Debugger;
using UnityEngine;

namespace Assets.Scripts.AI.Paths
{
    public class PathGroup : MonoBehaviour
    {
        public GameObject MapPlatform;
        public GameObject Debugger;

        public int TeamGroup;

        private List<AStarPath> _pathGroup;
        private MyDebugger _debugger;

        private bool _doOnce;
        private bool _toggle;

        protected void Start()
        {   _pathGroup = new List<AStarPath>( GetComponentsInChildren<AStarPath>() );
            if( Debugger ) _debugger = Debugger.GetComponent<MyDebugger>();
        }

        protected void Update()
        {
            if (_doOnce) return;

            _doOnce = true;
            RecalculateAllPaths();
        }
        public bool RecalculateAllPaths()
        {
            foreach (var path in _pathGroup)
            {
                List<Vector3> compact;
                List<Vector3> detailed;
                path.Recalculate( out compact, out detailed, TeamGroup);
                if ( compact.Count == 0 && detailed.Count == 0 )
                {
                    return false;
                }

                if ( path.CompactPath.Count != compact.Count)
                {
                    path.PathCounter++;
                }
                else if (compact.Where((t, it) => t != path.CompactPath[it]).Any())
                {
                    path.PathCounter++;
                }
                path.SetAStarPath( compact, detailed );
            }


            if ( _debugger == null ) return true;
            _debugger.ClearDebugCubes( gameObject.name + " _pathObject Debug_" + _toggle );
            _toggle = !_toggle;
            foreach (var loc in _pathGroup.SelectMany(path => path.CompactPath))
            {
                _debugger.DebugCube(loc, Color.green, gameObject.name + " _pathObject Debug_" + _toggle, 5);
            }
            return true;
        }
    }
}
