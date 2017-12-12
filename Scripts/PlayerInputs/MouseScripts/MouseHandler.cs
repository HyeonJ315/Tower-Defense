using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.AI.Platform;
using Assets.Scripts.NetworkManagement;
using Assets.Scripts.Selectable;
using Assets.Scripts.TrackingDictionaries;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs.MouseScripts
{
    public class MouseHandler : MonoBehaviour
    {
        private readonly Color _screenRectColor  = new Color( 0.8f, 0.8f, 0.95f, 0.25f );
        private readonly Color _screenRectBorder = new Color( 0.8f, 0.8f, 0.95f        );

        private const int BorderThickness      = 2;
        private const int MaxSelectableObjects = 20;
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
            if ( GeneralRpcClient.Instance == null )
            {
                Debug.Log("General Rpc is not found?");
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

            
            #region Deselect the selected objects that are not detected.

            foreach (var selectableObject in _selectedObjects.Keys )
            {
                if( _detectedObjects.ContainsKey( selectableObject ) ) continue;
                var objectComponent = selectableObject.GetComponent<SelectableComponent>();
                objectComponent.IsSelected = false;
                objectComponent.IsFaded    = false;
            }
            _selectedObjects.Clear();
            
            #endregion

            #region Select the detected objects.

            foreach ( var selectableObject in _detectedObjects.Keys )
            {
                var selectableComponent = selectableObject.GetComponent<SelectableComponent>();
                if (_selectedObjects.Count < MaxSelectableObjects)
                {
                    selectableComponent.IsSelected = true;
                    selectableComponent.IsFaded = false;
                    _selectedObjects.Add(selectableObject, true);
                }
                else
                {
                    selectableComponent.IsSelected = false;
                    selectableComponent.IsFaded = false;
                }
            }

            #endregion
            
            _detectedObjects.Clear();
        }

        private void _selectionHandler()
        {
            // Highlight all objects within the selection box
            if (!_isSelecting) return;
            LinkedList<GameObject> linkedList;
            if (GeneralRpcClient.Instance.PlayerNumber == 0) return;
            if ( !TurretListTrackerDictionary.Instance.GetEntry( GeneralRpcClient.Instance.PlayerNumber, out linkedList ) ) return;
            foreach (var selectableObject in linkedList)
            {
                if ( _isWithinSelectionBounds( selectableObject ) )
                {
                    if (  _detectedObjects.ContainsKey( selectableObject ) ) continue;
                    var objectComponent = selectableObject.GetComponent<SelectableComponent>();
                    _detectedObjects.Add(selectableObject, true);
                    if ( _selectedObjects.ContainsKey(selectableObject) ) continue;
                    objectComponent.IsSelected = true;
                    objectComponent.IsFaded = true;
                }
                else
                {
                    if ( !_detectedObjects.ContainsKey( selectableObject )) continue;
                    var objectComponent = selectableObject.GetComponent<SelectableComponent>();
                    _detectedObjects.Remove(selectableObject);
                    if ( _selectedObjects.ContainsKey(selectableObject) ) continue;
                    objectComponent.IsSelected = false;
                    objectComponent.IsFaded = false;
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
            RectangleDrawer.Instance.DrawScreenRect      ( rect, _screenRectColor );
            RectangleDrawer.Instance.DrawScreenRectBorder( rect, _screenRectBorder, BorderThickness );
        }
        
        private bool _isWithinSelectionBounds( GameObject go )
        {
            if ( !_isSelecting ) return false;
            var cam = Camera.main;
            var viewportBounds =
                RectangleDrawer.Instance.GetViewportBounds( cam, _mousePosition1, Input.mousePosition );
            var spriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
            if ( spriteRenderer.sprite == null ) return false;
            var spriteTransform = spriteRenderer.transform;
            var spriteBounds    = spriteRenderer.sprite.bounds;

            var spriteBottomRight = cam.WorldToViewportPoint( spriteTransform.TransformPoint( spriteBounds.max ) );
            var spriteTopLeft     = cam.WorldToViewportPoint( spriteTransform.TransformPoint( spriteBounds.min ) );
            var spriteBottomLeft  = new Vector3( spriteBottomRight.x, spriteTopLeft.y    , 0 );
            var spriteTopRight    = new Vector3( spriteTopLeft.x    , spriteBottomRight.y, 0 );

            var viewportTopRight     = viewportBounds.max;
            var viewportBottomLeft   = viewportBounds.min;

            return spriteBottomLeft.x   <= viewportTopRight.x && 
                   spriteBottomLeft.y   <= viewportTopRight.y && 
                   viewportBottomLeft.x <= spriteTopRight.x   && 
                   viewportBottomLeft.y <= spriteTopRight.y   ;
        }
    }
}