using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Assets.Scripts.NetworkManagement;
using Assets.Scripts.Selectable;
using Assets.Scripts.TrackingDictionaries;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.PlayerInputs.MouseScripts
{
    public class MouseHandler : MonoBehaviour
    {
        #region Singleton

        public static MouseHandler Instance { get; private set; }
        protected void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        #endregion

        private bool    _isSelecting;
        private Vector3 _mousePosition1;
        private readonly Dictionary<GameObject, bool> _selectedObjects = new Dictionary<GameObject, bool>();
        private readonly Dictionary<GameObject, bool> _detectedObjects = new Dictionary<GameObject, bool>();
        public IList<GameObject> GetSelectedObjects()
        {
            return _selectedObjects.Keys.ToList().AsReadOnly();
        }

        protected void Start()
        {
        }

        private void _leftMouseDown()
        {
            if ( !Input.GetMouseButtonDown(0) ) return;
            if ( PlayerState.Instance == null )
            {
                Debug.Log("Player State is not found?");
                return;
            }
            _isSelecting = true;
            _mousePosition1 = Input.mousePosition;
        }

        private void _leftMouseUp()
        {
            // If we let go of the left mouse button, end selection
            if ( !Input.GetMouseButtonUp(0) ) return;
            _isSelecting = false;

            #region Deselect the selected objects

            foreach (var selectableObject in _selectedObjects)
            {
                if( _detectedObjects.ContainsKey( selectableObject.Key ) ) continue;
                var objectComponent = selectableObject.Key.GetComponent<SelectableComponent>();
                objectComponent.IsSelected = false;
                objectComponent.IsFaded    = false;
            }
            _selectedObjects.Clear();

            #endregion

            #region Select the detected objects

            foreach ( var selectableObject in _detectedObjects.Keys )
            {
                selectableObject.GetComponent<SelectableComponent>().IsFaded = false;
                _selectedObjects.Add( selectableObject, true );
            }

            #endregion
            Debug.Log( _selectedObjects.Count + " Selected." );
            _detectedObjects.Clear();
            //TODO: WRITE THE SELECTION IN DEBUG
        }

        private void _selectionHandler()
        {
            // Highlight all objects within the selection box
            if (!_isSelecting) return;
            LinkedList<GameObject> linkedList;
            if ( !TurretListTrackerDictionary.Instance.GetEntry( NetworkingManager.Instance.client.connection.connectionId, out linkedList ) ) return;
            foreach (var selectableObject in linkedList)
            {
                if ( IsWithinSelectionBounds(selectableObject) )
                {
                    if ( _detectedObjects.ContainsKey(selectableObject) ) continue;
                    var objectComponent = selectableObject.GetComponent<SelectableComponent>();
                    if ( !objectComponent.IsSelected )
                    {
                        objectComponent.IsSelected = true;
                        objectComponent.IsFaded    = true;
                    }
                    _detectedObjects.Add(selectableObject, true);
                }
                else
                {
                    if ( !_detectedObjects.ContainsKey(selectableObject) ) continue;
                    var objectComponent = selectableObject.GetComponent<SelectableComponent>();
                    if ( objectComponent.IsSelected && objectComponent.IsFaded )
                    {
                        objectComponent.IsSelected = false;
                        objectComponent.IsFaded    = false;
                    }
                    _detectedObjects.Remove( selectableObject );
                }
            }
        }
        protected void Update()
        {
            if (!NetworkingManager.Instance.IsClient) return;
            _leftMouseDown();
            _leftMouseUp  ();
            _selectionHandler();
        }

        protected void OnGUI()
        {
            if ( !_isSelecting ) return;
            // Create a rect from both mouse positions
            var rect = RectangleDrawer.Instance.GetScreenRect( _mousePosition1, Input.mousePosition );
            RectangleDrawer.Instance.DrawScreenRect( rect, new Color(0.8f, 0.8f, 0.95f, 0.25f ) );
            RectangleDrawer.Instance.DrawScreenRectBorder( rect, 2, new Color(0.8f, 0.8f, 0.95f ) );
        }

        public bool IsWithinSelectionBounds( GameObject go )
        {
            if (!_isSelecting)
                return false;

            var cam = Camera.main;
            var viewportBounds =
                RectangleDrawer.Instance.GetViewportBounds( cam, _mousePosition1, Input.mousePosition );

            return viewportBounds.Contains( cam.WorldToViewportPoint(go.transform.position ) );
        }
    }
}