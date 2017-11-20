using System;
using System.Collections.Generic;

namespace Assets.Scripts.TurretScripts.TurretData
{
    // stores all string data that represents a prefab in the prefab directory.
    public class TurretCategoryDictionary
    {
        public static readonly string IconPrefabsDirectory = "Prefabs/Icons/";
        public static readonly string IconTag = "_Icon";

        public static readonly List< string > CategoryTypeList = new List<string>( Enum.GetNames( typeof(TurretCategoryType) ) );
        // Add more to these lists to place more towers. Be sure to add the prefab and prefab_Icon in the icon prefab directory.
        public static readonly Dictionary< TurretCategoryType, List<string> > Instance = new Dictionary< TurretCategoryType, List< string > >()
        {
            {   TurretCategoryType.Dark, new List<string>()
                {
                    "Beginner_Dark0",
                }
            },
            {   TurretCategoryType.Earth, new List<string>()
                {
                    "Beginner_Earth0",
                }
            },
            {   TurretCategoryType.Fire, new List<string>()
                {
                    "Beginner_Fire0",
                }
            },
            {   TurretCategoryType.Ice, new List<string>()
                {
                    "Beginner_Ice0",
                }
            },
            {   TurretCategoryType.Light, new List<string>()
                {
                    "Beginner_Light0",
                }
            },
            {   TurretCategoryType.Poison, new List<string>()
                {
                    "Beginner_Poison0",
                }
            },
            {   TurretCategoryType.Thunder, new List<string>()
                {
                    "Beginner_Thunder0",
                }
            },
            {   TurretCategoryType.Water, new List<string>()
                {
                    "Beginner_Water0",
                }
            },
        };
    }
}
