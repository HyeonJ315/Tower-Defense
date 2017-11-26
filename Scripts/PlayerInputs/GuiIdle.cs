using System.Linq;
using Assets.Scripts.PlayerInputs.MouseScripts;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs
{
    public class GuiIdle : Gui
    {
        // Use this for initialization
        protected override void Start()
        {
            SetButton( "SelectButton", Button_Select, "" );
            SetButton( "BuildButton" , Button_Build , "" );
            SetButton( "SpawnButton" , Button_Spawn , "" );
            SetButton( "SellButton"  , Button_Sell  , "" );
            SetButton( "MenuButton"  , Button_Menu  , "" );
        }

        // Update is called once per frame
        protected override void Update()
        {
            if      (Input.GetKeyDown("1"))
                Button_Select("");
            else if (Input.GetKeyDown("2"))
                Button_Build("");
            else if (Input.GetKeyDown("3"))
                Button_Spawn("");
            else if (Input.GetKeyDown("4"))
                Button_Sell("");
            else if (Input.GetKeyDown("5"))
                Button_Menu("");
        }

        private void Button_Select(string msg)
        {
            ReplaceMeWith("GUI_Select");
        }

        private void Button_Build(string msg)
        {
            ReplaceMeWith("GUI_Build");
        }

        private void Button_Spawn(string msg)
        {
            ReplaceMeWith("GUI_Spawn");
        }

        private void Button_Sell(string msg)
        {
            ReplaceMeWith("GUI_Selling");
        }

        private void Button_Menu(string msg)
        {
            ReplaceMeWith("GUI_Menu");
        }
    }
}
