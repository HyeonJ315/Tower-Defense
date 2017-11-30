using UnityEngine;

namespace Assets.Scripts.Selectable
{
    internal class SelectableComponent : MonoBehaviour
    {
        public string PrefabDir           = "Turrets"             ;
        public string SelectionCircle     = "SelectionCircle"     ;
        public string MaterialsDir        = "Materials/Projection";
        public string SelectMaterial      = "SelectMaterial"      ;
        public string SelectMaterialFaded = "SelectMaterialFaded" ;
        public bool   IsSelected;
        public bool   IsFaded;
        private bool _selected;
        private bool _faded;
        private GameObject _circleGameObject;
        protected void Start()
        {
        }

        protected void Update()
        {
            if ( IsSelected && !_selected )
            {
                _circleGameObject = Instantiate( Resources.Load( PrefabDir + "/" + SelectionCircle ) ) as GameObject;
                if ( _circleGameObject == null ) return;
                _circleGameObject.transform.SetParent(transform);
                _circleGameObject.transform.localPosition = _circleGameObject.transform.position + Vector3.zero;
                _selected = true;
                
            }
            else if ( !IsSelected && _selected )
            {
                if (_circleGameObject == null) return;
                Destroy( _circleGameObject );
                _selected = false;
            }

            if ( IsFaded && !_faded )
            {
                if ( _circleGameObject == null ) return;
                var currColor = _circleGameObject.GetComponent<Projector>().material.color;
                currColor.a = 0.5f;
                _circleGameObject.GetComponent<Projector>().material.SetColor( "_Color", currColor );
                _faded = true;  
            }
            else if ( !IsFaded && _faded )
            {
                if (_circleGameObject == null) return;
                var currColor = _circleGameObject.GetComponent<Projector>().material.color;
                currColor.a = 1.0f;
                _circleGameObject.GetComponent<Projector>().material.SetColor( "_Color", currColor );
                _faded = false;
            }
        }
    }
}
