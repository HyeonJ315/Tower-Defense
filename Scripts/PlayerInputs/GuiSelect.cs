using System.Linq;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs
{
    public class GuiSelect : Gui
    {
        // Use this for initialization
        protected override void Start ()
        {
            SetButton( "BackButton", Button_Back, "" );
            SetButton( "SwapButton", Button_Swap, "" );
            SetButton( "AttackButton", Button_Attack, "" );
            SetButton( "CastButton", Button_Cast, "" );
        }

        // Update is called once per frame
        protected override void Update ()
        {
            if (Input.GetKeyDown("1"))
            {
                Button_Back("");
            }
            else if (Input.GetKeyDown("2"))
            {
                Button_Swap("");
            }
            else if (Input.GetKeyDown("3"))
            {
                
            }
            else if (Input.GetKeyDown("4"))
            {
                
            }

        }

        private void Button_Back( string msg )
        {
            ReplaceMeWith( "GUI_Idle" );
        }

        private void Button_Swap(string msg)
        {
            ReplaceMeWith( "GUI_SelectSwap" );
        }

        private void Button_Attack(string msg)
        {
            
        }

        private void Button_Cast(string msg)
        {
            
        }
    }
}
