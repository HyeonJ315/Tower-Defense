using System;
using System.Collections.Generic;
using Assets.Scripts.MobScripts.MobData;
using Assets.Scripts.MobScripts.MobManagement;
using Assets.Scripts.NetworkManagement;
using UnityEngine;

namespace Assets.Scripts.PlayerInputs
{
    public class GuiSpawn : Gui
    {
        private MobManagerRpc _mobManagerRpc;
        // Use this for initialization
        protected override void Start ()
        {
            _mobManagerRpc = MobManagerRpcClient.Instance;
            SetButton("ButtonBack", Button_Back, "");
            _loadScrollList(
                "CategoryListGrid", 
                MobCategoryDictionary.IconPrefabsDirectory, 
                MobCategoryDictionary.IconTag,
                MobCategoryDictionary.CategoryTypeList, 
                CategoryReceiver );
        }   

        // Update is called once per frame
        protected override void Update ()
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
            _unloadScrollList( "SubListGrid" );
            var category = (MobCategoryType) Enum.Parse(typeof(MobCategoryType), msg, true);
            List<string> subListGrid;
            MobCategoryDictionary.Instance.TryGetValue( category, out subListGrid );
            _loadScrollList(
                "SubListGrid", 
                MobCategoryDictionary.IconPrefabsDirectory,
                MobCategoryDictionary.IconTag,
                subListGrid,
                SublistReceiver);
        }

        private void SublistReceiver(string msg)
        {
            if ( !Enum.IsDefined(typeof(MobPrefabs), msg.Split('_')[0]) )
                return;
            var mobNumber = (MobPrefabs)Enum.Parse(typeof(MobPrefabs), msg.Split('_')[0], true);
            _mobManagerRpc.MobSpawnSendRpc( (int) mobNumber, 1 );
        }
    }
}
