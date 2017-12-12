using System;
using System.Collections.Generic;
using Assets.Scripts.FileUtilities;
using UnityEngine;

namespace Assets.Scripts.TurretScripts.TurretData
{
    internal class TurretRepository
    {
        public const string TurretDir = "Turrets";
        private const string AttributesFile = "Attributes";

        #region Singleton

        private static readonly TurretRepository I = new TurretRepository();
        static TurretRepository() { }
        private TurretRepository() { }
        public static TurretRepository Instance { get { return I; } }

        #endregion

        public bool Initialized { private set; get; }

        public readonly Dictionary<string, int>              TurretNameToId             = new Dictionary<string, int>();
        public readonly Dictionary<string, TurretAttributes> TurretNameToAttributes     = new Dictionary<string, TurretAttributes>();
        public readonly Dictionary<int, string>              TurretIdToName             = new Dictionary<int, string>();
        public readonly Dictionary<int, TurretAttributes>    TurretIdToAttributes       = new Dictionary<int, TurretAttributes>();
        public readonly Dictionary<string, List<int>>        TurretTypeToId             = new Dictionary<string, List<int>>();
        public readonly Dictionary<string, List<string>>     TurretTypeToName           = new Dictionary<string, List<string>>();
        public readonly Dictionary<string, List<string>>     TurretTypeToFullName       = new Dictionary<string, List<string>>();
        public readonly Dictionary<string, TurretAttributes> TurretFullNameToAttributes = new Dictionary<string, TurretAttributes>();

        public bool Initialize()
        {
            if (Initialized)
            {
                Debug.LogWarning("Turret Dictionary is already initialized!");
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

                #region populate folders with string entries of all folders in Resources/Turrets

                var tmpDictionary = new Dictionary< string, bool >();
                foreach ( var text in manifestText )
                {
                    var textSplit = text.Split('\\', '/');
                    if (textSplit.Length == 1) continue;
                    if ( textSplit[0] != TurretDir ) continue;
                    if (tmpDictionary.ContainsKey(textSplit[1])) continue;
                    folders.Add( textSplit[1] );
                    tmpDictionary.Add( textSplit[1], true );
                }
                tmpDictionary.Clear();

                #endregion

                foreach (var folder in folders)
                {
                    #region Obtain the text asset and find the turret name and Id.

                    var attributeList = Resources.Load( TurretDir + "/" + folder + "/" + AttributesFile ) as TextAsset;
                    if (!attributeList)
                    {
                        Debug.Log( "Could not load the attribute file of " + folder );
                        return false;
                    }
                    var turretAttributes = new TurretAttributes();
                    var turretId = int.Parse(folder.Split('_')[0]);
                    var turretName = folder.Split('_')[1];

                    #endregion

                    TurretNameToId.Add( turretName, turretId);
                    TurretIdToName.Add( turretId, turretName);

                    #region Get parse the turret's attributes and store into an attribute object and referenced by a dictionary.

                    var attributeStrings = attributeList.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    foreach (var attributeString in attributeStrings )
                    {
                        var tmp = attributeString.Trim().Replace(" ", "");
                        var tmp2 = tmp.Split(':');
                        if (tmp2.Length == 1) continue;
                        var attribute = tmp2[0];
                        var value = tmp2[1];
                        switch (attribute)
                        {
                            case "ProjectileNumber":
                                turretAttributes.ProjectileNumber = int.Parse(value); break;
                            case "AttackAir":
                                turretAttributes.AttackAir = float.Parse(value); break;
                            case "AttackGround":
                                turretAttributes.AttackGround = float.Parse(value); break;
                            case "AttackSpeed":
                                turretAttributes.AttackSpeed = int.Parse(value); break;
                            case "AttackElement":
                                turretAttributes.AttackElement = value; break;
                            case "ProjectileSpeed":
                                turretAttributes.ProjectileSpeed = float.Parse(value); break;
                            case "Splash":
                                turretAttributes.Splash = float.Parse( value ); break;
                            case "Range":
                                turretAttributes.Range = float.Parse(value); break;
                            case "Cost":
                                turretAttributes.Cost = uint.Parse(value); break;
                            case "ProjectileSpawnOffsetX":
                                turretAttributes.ProjectileSpawnOffset.x = float.Parse( value ); break;
                            case "ProjectileSpawnOffsetY":
                                turretAttributes.ProjectileSpawnOffset.y = float.Parse( value ); break;
                            case "ProjectileSpawnOffsetZ":
                                turretAttributes.ProjectileSpawnOffset.z = float.Parse( value ); break;
                            case "Type":
                                if (turretAttributes.Types[0] == null) turretAttributes.Types[0] = value;
                                else if (turretAttributes.Types[1] == null) turretAttributes.Types[1] = value;
                                break;
                        }
                    }

                    #endregion

                    TurretIdToAttributes.Add(turretId, turretAttributes);
                    TurretNameToAttributes.Add(turretName, turretAttributes);
                    TurretFullNameToAttributes.Add( "" + turretId + "_" + turretName, turretAttributes );

                    #region Set the turret type-to-name/id dictionary.

                    if (turretAttributes.Types[0] != null)
                    {
                        List<int> turretIdList;
                        List<string> turretNameList;
                        if (TurretTypeToId.TryGetValue(turretAttributes.Types[0], out turretIdList))
                            turretIdList.Add(turretId);
                        else
                            TurretTypeToId.Add(turretAttributes.Types[0], new List<int> { turretId });

                        if (TurretTypeToName.TryGetValue(turretAttributes.Types[0], out turretNameList))
                            turretNameList.Add(turretName);
                        else
                            TurretTypeToName.Add(turretAttributes.Types[0], new List<string> { turretName });
                        if (TurretTypeToFullName.TryGetValue(turretAttributes.Types[0], out turretNameList))
                            turretNameList.Add("" + turretId + "_" + turretName);
                        else
                            TurretTypeToFullName.Add(turretAttributes.Types[0], new List<string> { "" + turretId + "_" + turretName });
                    }
                    if (turretAttributes.Types[1] != null)
                    {
                        List<int> turretIdList;
                        List<string> turretNameList;
                        if (TurretTypeToId.TryGetValue(turretAttributes.Types[1], out turretIdList))
                            turretIdList.Add(turretId);
                        else
                            TurretTypeToId.Add(turretAttributes.Types[1], new List<int> { turretId });

                        if (TurretTypeToName.TryGetValue(turretAttributes.Types[1], out turretNameList))
                            turretNameList.Add(turretName);
                        else
                            TurretTypeToName.Add(turretAttributes.Types[1], new List<string> { turretName });
                        if (TurretTypeToFullName.TryGetValue(turretAttributes.Types[1], out turretNameList))
                            turretNameList.Add("" + turretId + "_" + turretName);
                        else
                            TurretTypeToFullName.Add(turretAttributes.Types[1], new List<string> { "" + turretId + "_" + turretName });
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                Debug.LogError( e.StackTrace );
                _clearDictionaries();
                return false;
            }
            return Initialized = true;
        }

        private void _clearDictionaries()
        {
            TurretNameToId.Clear();
            TurretNameToAttributes.Clear();
            TurretIdToName.Clear();
            TurretIdToAttributes.Clear();
            TurretTypeToId.Clear();
            TurretTypeToName.Clear();
            TurretTypeToFullName.Clear();
            TurretFullNameToAttributes.Clear();
        }
    }
}
