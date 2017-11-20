using System;
using System.Collections.Generic;

namespace Assets.Scripts.MobScripts.MobData
{
    // stores all string data that represents a prefab in the prefab directory.
    public class MobCategoryDictionary
    {
        public static readonly string IconPrefabsDirectory = "Prefabs/Icons/";
        public static readonly string IconTag = "_Icon";

        public static readonly List<string> CategoryTypeList = new List<string>(Enum.GetNames(typeof(MobCategoryType)));
        // Add more to these lists to spawn more types of monsters. Be sure to add the prefab and prefab_Icon in the icon prefab directory.
        public static readonly Dictionary<MobCategoryType, List<string>> Instance = new Dictionary<MobCategoryType, List<string>>()
        {
            {   MobCategoryType.Tier1, new List<string>()
                {
                    "TestMob",
                }
            },
            {   MobCategoryType.Tier2, new List<string>()
                {
                }
            },
            {   MobCategoryType.Tier3, new List<string>()
                {
                }
            },
            {   MobCategoryType.Tier4, new List<string>()
                {
                }
            },
            {   MobCategoryType.Tier5, new List<string>()
                {
                }
            },
            {   MobCategoryType.Tier6, new List<string>()
                {
                }
            },
            {   MobCategoryType.Tier7, new List<string>()
                {
                }
            },
            {   MobCategoryType.Tier8, new List<string>()
                {
                }
            },
            {   MobCategoryType.Tier9, new List<string>()
                {
                }
            },
        };
    }
}
