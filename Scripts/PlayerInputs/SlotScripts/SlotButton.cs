using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerInputs.SlotScripts
{
    public class SlotButton : Button
    {
        public int ButtonId { get; private set; }
        public bool Bound;
        public string Description;
        protected override void Start()
        {
            base.Start();
            ButtonId = Convert.ToInt32( name.Split('_')[1] );
        }

        public override void OnPointerEnter( PointerEventData eventData )
        {
            base.OnPointerEnter( eventData );
            if (Bound)
            {
                SlotDescription.Instance.TextComponent.text = Description;
                SlotDescription.Instance.EnableActiveState ();
            }
            else
                SlotDescription.Instance.DisableActiveState();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit( eventData );
            SlotDescription.Instance.DisableActiveState();
        }
    }
}
