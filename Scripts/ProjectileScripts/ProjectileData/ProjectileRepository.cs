using System;
using System.Collections.Generic;
using Assets.Scripts.FileUtilities;
using UnityEngine;

namespace Assets.Scripts.ProjectileScripts.ProjectileData
{
    internal class ProjectileRepository
    {
        #region Singleton

        private static readonly ProjectileRepository I = new ProjectileRepository();
        static ProjectileRepository() { }
        private ProjectileRepository() { }
        public static ProjectileRepository Instance { get { return I; } }

        #endregion

        public const string ProjectileDir = "Projectiles";

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

                    var projectileId   = int.Parse(folder.Split('_')[0]);
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
