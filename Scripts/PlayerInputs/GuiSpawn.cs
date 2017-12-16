/*
using System;
using System.Collections.Generic;
using Assets.Scripts.ElementScripts;
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
                "ElementTypes/", ElementRepository.Instance.ElementFullNameToAttributes.Keys, "/Icon",
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

            List<string> subListGrid;
            MobRepository.Instance.MobTypeToFullName.TryGetValue( msg.Split('_')[1], out subListGrid );
            _loadScrollList(
                "SubListGrid", 
                "Mobs/", subListGrid, "/Icon",
                SublistReceiver );
        }

        private void SublistReceiver(string msg)
        {
            int mobNumber;
            if( MobRepository.Instance.MobNameToId.TryGetValue( msg.Split('_')[1], out mobNumber ) )
                _mobManagerRpc.MobSpawnSendRpc( mobNumber, 1 );
        }
    }
}
*/