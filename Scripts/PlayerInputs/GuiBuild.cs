using System;
using System.Collections.Generic;
using System.Linq;
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
            ReplaceMeWith("GUI_Idle");
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
                SublistReceiver );
        }

        private void SublistReceiver(string msg)
        {
            var nextGui = ReplaceMeWith( "GUI_Building" );
            nextGui.GetComponent<GuiBuilding>().CurrentSpawningTurret = msg;
        }
    }
}