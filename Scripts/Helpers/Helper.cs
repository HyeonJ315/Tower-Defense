
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public class Helper : MonoBehaviour
    {   
        public GameObject GuiHelper;
        public GameObject TurretsHelper;
        public GameObject MobsHelper;
        public GameObject ProjectileHelper;
        public GameObject RtsCamera;
        protected void Start()
        {
            GuiHelper        = GameObject.Find( "GUIHelper"         );
            TurretsHelper    = GameObject.Find( "TurretsHelper"     );
            MobsHelper       = GameObject.Find( "MobsHelper"        );
            ProjectileHelper = GameObject.Find( "ProjectilesHelper" );
            RtsCamera        = GameObject.Find( "RTS_Camera"        );
        }
    }
}