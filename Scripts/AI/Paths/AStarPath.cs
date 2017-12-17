using System.Collections.Generic;
using Assets.Scripts.AI.Platform;
using UnityEngine;

namespace Assets.Scripts.AI.Paths
{
    public class AStarPath : MonoBehaviour
    {
        public static Dictionary< string, GameObject > AStarPathDictionary = new Dictionary<string, GameObject>();

        public GameObject StartPoint,
                          EndPoint;
        public GameObject NextPath;

        public ulong PathCounter;

        private MapPlatform   _mapPlatform;

        public List<Vector3> CompactPath  { get; private set; }
        public List<Vector3> DetailedPath { get; private set; }

        protected void Start()
        {       
            CompactPath  = new List<Vector3>();
            DetailedPath = new List<Vector3>();
            AStarPathDictionary.Add( gameObject.name, gameObject );
            _mapPlatform = MapPlatform.Instance;
        }

        protected void OnDestroy()
        {
            AStarPathDictionary.Remove( gameObject.name );
        }

        public void SetAStarPath(List<Vector3> compact, List<Vector3> detailed)
        {
            if (compact == null)
            {
                Debug.LogWarning("Compact was null and the path could not be set.");
            }
            if (detailed == null)
            {
                Debug.LogWarning("Detailed was null and the path could not be set.");
            }

            CompactPath  = compact;
            DetailedPath = detailed;
        }

        public void Recalculate( out List<Vector3> compact, out List<Vector3> detailed, int teamGroup = 0 )
        {   switch (teamGroup)
            {
                case 1:
                    _mapPlatform.FindAStarPath(StartPoint.transform.position,
                                               EndPoint.transform.position,
                                               out compact,
                                               out detailed,
                                               false,
                                               true
                                              );
                    break;
                case 2:
                    _mapPlatform.FindAStarPath(StartPoint.transform.position,
                                               EndPoint.transform.position,
                                               out compact,
                                               out detailed,
                                               true
                                              );
                    break;
                default:
                    _mapPlatform.FindAStarPath(StartPoint.transform.position,
                                               EndPoint.transform.position,
                                               out compact,
                                               out detailed
                                              );
                    break;
            }
        }
    }
}