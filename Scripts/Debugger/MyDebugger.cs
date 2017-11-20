using UnityEngine;

namespace Assets.Scripts.Debugger
{
    public class MyDebugger : MonoBehaviour
    {

        public void DebugCube(Vector3 loc, Color c, string goName,int scale = 1 )
        {
            var debugger = GameObject.Find( goName );
            if ( debugger == null)
            {
                debugger = new GameObject { name = goName };
                debugger.transform.SetParent( gameObject.transform );
            }
            
            Debug.Log("DebugCube");
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = loc;
            go.transform.Translate(Vector3.up);
            go.transform.localScale = Vector3.one / scale;
            go.GetComponent<MeshRenderer>().material.color = c;
            go.transform.SetParent(debugger.transform);
        }

        public void ClearDebugCubes( string goName )
        {
            var go = GameObject.Find( goName );
            Destroy( go );
        }
    }
}
