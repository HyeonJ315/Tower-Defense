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
        public GameObject  Image;
        public string Description;

        public SlotButtonStruct( List<UnityAction> actionList, GameObject image, string description = "" )
        {
            ActionList  = actionList;
            Image       = image;
            Description = description;
        }
    }
}
