using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerInputs.SlotScripts
{
    public class SlotButtonStruct
    {
        public List< UnityAction > ActionList;
        public List< UnityAction<object> > ActionListPara;
        public GameObject  ImagePrefab;
        public string Description;

        public SlotButtonStruct( List<UnityAction> actionList, GameObject imagePrefab, string description = "" )
        {
            ActionList  = actionList;
            ImagePrefab = imagePrefab;
            Description = description;
        }
    }
}
