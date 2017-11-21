
using UnityEditor;

namespace Assets.Scripts.FileUtilities
{
    class ManifestMenuItem
    {
        [MenuItem("Manifest/Create Manifest %M")]
        public static void CreateManifest()
        {
            ManifestManager.CreateManifest();
        }
    }
}
