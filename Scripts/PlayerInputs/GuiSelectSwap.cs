/*
using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.ElementScripts;
using Assets.Scripts.TurretScripts.TurretData;

namespace Assets.Scripts.PlayerInputs
{
    public class GuiSelectSwap : Gui
    {
        // Use this for initialization
        protected override void Start()
        {
            SetButton("SelectButton", Button_Back, "");
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
            ReplaceMeWith("GUI_Select");
        }

        private void CategoryReceiver(string msg)
        {
            _unloadScrollList("SubListGrid");

            List<string> subListGrid;
            if (!TurretRepository.Instance.TurretTypeToFullName.TryGetValue(msg.Split('_')[1], out subListGrid))
                return;
            _loadScrollList(
                "SubListGrid",
                "Turrets/", subListGrid, "/Icon",
                SublistReceiver);
        }

        private void SublistReceiver(string msg)
        {
            var nextGui = ReplaceMeWith("GUI_Building");
            nextGui.GetComponent<GuiBuilding>().CurrentSpawningTurret = msg;
        }
    }
}
*/