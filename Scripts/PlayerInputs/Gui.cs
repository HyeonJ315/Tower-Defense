using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerInputs
{
    public class Gui : MonoBehaviour
    {
        protected delegate void ReceiverDelegate(string msg);
        public static readonly string GuiPrefabsDirectory = "GUI";
        public static readonly string PlayerName          = "PlayerName";
        public static readonly string CameraString        = "RtsCamera";
        public static readonly int    TeamGroup           = 2;
        protected readonly Dictionary<string, GameObject> InteractableGoDictionary = new Dictionary<string, GameObject>();
        public delegate void ButtonOnClick(string message);
        public List<string> TargetsTags;

        protected static Helper Helper;
        protected ButtonOnClick OnClickButton;

        protected virtual void Awake()
        {
            SetInteractableDictionary();
            if (Helper) return;
            Helper = GameObject.Find("ClientHelpers").GetComponent<Helper>();
        }

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            
        }

        protected void SetInteractableDictionary()
        {
            var transforms = GetComponentsInChildren<Transform>();
            foreach (var t in transforms)
            {
                GameObject go;
                if (InteractableGoDictionary.TryGetValue(t.name, out go)) continue;
                InteractableGoDictionary.Add(t.name, t.gameObject);
            }
        }

        protected GameObject ReplaceMeWith( string replacingPrefab )
        {
            var resource = Resources.Load( GuiPrefabsDirectory + "/" + replacingPrefab );
            if (!resource) return null;
            var guiGameObject = Instantiate( resource ) as GameObject;
            if (!guiGameObject) return null;
            guiGameObject.transform.SetParent( Helper.GuiHelper.transform );
            DestroyObject( gameObject );
            return guiGameObject;
        }

        protected bool SetButton( string buttonGoString, ButtonOnClick boc, string message )
        {
            GameObject currentGameObject;
            if ( !InteractableGoDictionary.TryGetValue("SelectButton", out currentGameObject) ) return false;
            var button = currentGameObject.GetComponent<Button>();
            button.onClick.AddListener( delegate { boc(message); } );
            return true;
        }

        #region Helper methods for scroll lists

        // loads data into the scroll list grid with game object named scrolllistgrid, prefab directory dir, list of all icons categoryList, and a tag.
        protected void _loadScrollList(string scrollListGrid, string prefabsDir, IEnumerable<string> categoryList, string iconName, ReceiverDelegate recv )
        {
            var componentList = GetComponentsInChildren<RectTransform>();

            GameObject categoryListGrid = null;

            #region search for the category list grid in the current gui 

            foreach (var component in componentList.Where(component => component.name == scrollListGrid))
            {
                categoryListGrid = component.gameObject;
            }
            if (categoryListGrid == null)
            {
                Debug.LogWarning("Scroll list could not be loaded.");
                return;
            }

            #endregion

            #region place all categories in the category list.

            if (categoryList == null) return;
            foreach (var category in categoryList )
            {
                var go = Instantiate( Resources.Load( prefabsDir + category + iconName ) ) as GameObject;
                if (go == null)
                {
                    Debug.LogWarning("Scroll list could not be loaded.");
                    _unloadScrollList(scrollListGrid);
                    return;
                }
                go.name = category + "_Icon";
                go.transform.SetParent(categoryListGrid.transform);
                var categoryClone = category;
                go.GetComponent<Button>().onClick.AddListener(delegate { recv(categoryClone); });
                go.transform.localScale = Vector3.one;
            }

            #endregion
        }

        // unloads all data from the scroll list grid with the game object named scrolllistgrid.
        protected void _unloadScrollList(string scrollListGrid)
        {
            var componentList = GetComponentsInChildren<RectTransform>();

            GameObject categoryListGrid = null;

            #region search for the category list grid in the current gui

            foreach (var component in componentList.Where(component => component.name == scrollListGrid))
            {
                categoryListGrid = component.gameObject;
            }
            if (categoryListGrid == null)
            {
                Debug.LogWarning( "Scroll list does not even exist." );
                return;
            }

            #endregion

            var categoryList = categoryListGrid.GetComponentsInChildren<RectTransform>();

            #region destroy categories in the category list 

            foreach (var category in categoryList.Where(category => category.gameObject != categoryListGrid))
            {
                Destroy(category.gameObject);
            }

            #endregion
        }

        #endregion

    }
}
