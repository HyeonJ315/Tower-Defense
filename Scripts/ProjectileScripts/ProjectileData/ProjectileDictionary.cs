using System;
using System.Collections.Generic;
using Assets.Scripts.FileUtilities;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    internal class ProjectileDictionary
    {
        public const string ProjectileDir = "ProjectileTypes";
        private const string AttributesFile = "Attributes";

        #region Singleton

        private static readonly ProjectileDictionary I = new ProjectileDictionary();
        static ProjectileDictionary() { }
        private ProjectileDictionary() { }
        public static ProjectileDictionary Instance { get { return I; } }

        #endregion

        public bool Initialized { private set; get; }

        public readonly Dictionary<string, int> ProjectileNameToId = new Dictionary<string, int>();
        public readonly Dictionary<int, string> ProjectileIdToName = new Dictionary<int, string>();

        public bool Initialize()
        {
            if (Initialized)
            {
                Debug.LogWarning("Projectile Dictionary is already initialized!");
            }
            try
            {
                #region Attempt to open the manifest text file.

                var manifestText = ManifestManager.OpenManifest();
                if (manifestText == null)
                {
                    return false;
                }

                #endregion

                var folders = new List<string>();

                #region populate folders with string entries of all folders in Resources/Projectiles

                var tmpDictionary = new Dictionary<string, bool>();
                foreach (var text in manifestText)
                {
                    var textSplit = text.Split('\\', '/');
                    if (textSplit.Length == 1) continue;
                    if (textSplit[0] != ProjectileDir) continue;
                    if (tmpDictionary.ContainsKey(textSplit[1])) continue;
                    folders.Add(textSplit[1]);
                    tmpDictionary.Add(textSplit[1], true);
                }
                tmpDictionary.Clear();

                #endregion

                foreach (var folder in folders)
                {
                    #region Obtain the text asset and find the projectile name and Id.

                    var attributeList = Resources.Load(ProjectileDir + "/" + folder + "/" + AttributesFile) as TextAsset;
                    if (!attributeList)
                    {
                        Debug.Log("Could not load the attribute file of " + folder);
                        return false;
                    }
                    var projectileId = int.Parse(folder.Split('_')[0]);
                    var projectileName = folder.Split('_')[1];

                    #endregion

                    ProjectileNameToId.Add(projectileName, projectileId);
                    ProjectileIdToName.Add(projectileId, projectileName);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
                _clearDictionaries();
                return false;
            }
            return Initialized = true;
        }

        private void _clearDictionaries()
        {
            ProjectileNameToId.Clear();
            ProjectileIdToName.Clear();
        }
    }
}
