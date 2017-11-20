using System;
using UnityEngine;
using System.Collections.Generic;
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
                TurretCategoryDictionary.IconPrefabsDirectory,
                TurretCategoryDictionary.IconTag,
                TurretCategoryDictionary.CategoryTypeList,
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
            var category = (TurretCategoryType)Enum.Parse(typeof(TurretCategoryType), msg, true);
            List<string> subListGrid;
            TurretCategoryDictionary.Instance.TryGetValue(category, out subListGrid);
            _loadScrollList(
                "SubListGrid",
                TurretCategoryDictionary.IconPrefabsDirectory,
                TurretCategoryDictionary.IconTag,
                subListGrid,
                SublistReceiver);
        }

        private void SublistReceiver(string msg)
        {
            var nextGui = ReplaceMeWith("GUI_Building");
            nextGui.GetComponent<GuiBuilding>().CurrentSpawningTurret = msg;
        }
    }
}
