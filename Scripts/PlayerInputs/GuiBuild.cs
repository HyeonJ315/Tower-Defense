using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.ElementScripts;
using Assets.Scripts.TurretScripts.TurretData;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerInputs
{
    public class GuiBuild : Gui
    {
        protected override void Start()
        {
            SetButton("BackButton", Button_Back, "");
            _loadScrollList(
                "CategoryListGrid",
                "ElementTypes/", ElementRepository.Instance.ElementFullNameToAttributes.Keys, "/Icon",
                CategoryReceiver );
        }

        // Update is called once per frame
        protected override void Update()
        {
            if (Input.GetKeyDown("1"))
                Button_Back("");

        }

        private void Button_Back(string msg)
        {
            ReplaceMeWith("GUI_Idle");
        }

        private void CategoryReceiver(string msg)
        {
            _unloadScrollList("SubListGrid");
            List<string> subListGrid;
            if ( !TurretRepository.Instance.TurretTypeToFullName.TryGetValue( msg.Split('_')[1], out subListGrid) )
                return;
            _loadScrollList(
                "SubListGrid", 
                "Turrets/", subListGrid, "/Icon",
                SublistReceiver );
        }

        private void SublistReceiver(string msg)
        {
            var nextGui = ReplaceMeWith( "GUI_Building" );
            nextGui.GetComponent<GuiBuilding>().CurrentSpawningTurret = msg.Split('_')[1];
        }
    }
}