using System;
using System.Collections.Generic;
using Assets.Scripts.FileUtilities;
using UnityEngine;

namespace Assets.Scripts.MobScripts.MobData
{
    internal class MobRepository
    {
        #region Singleton

        private static readonly MobRepository I = new MobRepository();
        static MobRepository(){} private MobRepository(){}
        public static MobRepository Instance { get { return I; } }

        #endregion

        public const string MobDir = "Mobs";
        private const string AttributesFile = "Attributes";

        public bool Initialized { private set; get; }

        public readonly Dictionary< string, int           > MobNameToId             = new Dictionary< string, int >();
        public readonly Dictionary< string, MobAttributes > MobNameToAttributes     = new Dictionary< string, MobAttributes >();
        public readonly Dictionary< int   , string        > MobIdToName             = new Dictionary< int, string >();
        public readonly Dictionary< int   , MobAttributes > MobIdToAttributes       = new Dictionary< int, MobAttributes >();
        public readonly Dictionary< string, MobAttributes > MobFullNameToAttributes = new Dictionary< string, MobAttributes >();
        public readonly Dictionary< string, List<int>     > MobTypeToId             = new Dictionary< string, List< int > >();
        public readonly Dictionary< string, List<string>  > MobTypeToName           = new Dictionary< string, List< string > >();
        public readonly Dictionary< string, List<string>  > MobTypeToFullName       = new Dictionary< string, List< string > >();


        public bool Initialize()
        {
            if (Initialized)
            {
                Debug.LogWarning( "Mob Dictionary is already initialized!" );
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

                #region populate folders with string entries of all folders in Resources/Mobs

                var tmpDictionary = new Dictionary<string, bool>();
                foreach (var text in manifestText)
                {
                    var textSplit = text.Split('\\', '/');
                    if (textSplit.Length == 1) continue;
                    if (textSplit[0] != MobDir) continue;
                    if (tmpDictionary.ContainsKey(textSplit[1])) continue;
                    folders.Add(textSplit[1]);
                    tmpDictionary.Add(textSplit[1], true);
                }
                tmpDictionary.Clear();

                #endregion

                foreach (var folder in folders)
                {
                    #region Obtain the text asset and find the mob name and Id.

                    var attributeList = Resources.Load(MobDir + "/" + folder + "/" + AttributesFile) as TextAsset;
                    if (!attributeList)
                    {
                        Debug.Log("Could not load the attribute file of " + folder);
                        return false;
                    }
                    var mobAttributes = new MobAttributes();
                    var mobId = int.Parse(folder.Split('_')[0]);
                    var mobName = folder.Split('_')[1];

                    #endregion

                    MobNameToId.Add(mobName, mobId);
                    MobIdToName.Add(mobId, mobName);

                    #region Get parse the mob's attributes and store into an attribute object and referenced by a dictionary.

                    var attributeStrings =
                        attributeList.text.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
                    foreach (var attributeString in attributeStrings)
                    {
                        var tmp = attributeString.Trim().Replace(" ", "");
                        var tmp2 = tmp.Split(':');
                        if (tmp2.Length == 1) continue;
                        var attribute = tmp2[0];
                        var value = tmp2[1];
                        switch (attribute)
                        {
                            case "Health":
                                mobAttributes.Health = float.Parse(value); break;
                            case "Defense":
                                mobAttributes.Defense = float.Parse(value); break;
                            case "SpecialDefense":
                                mobAttributes.SpecialDefense = float.Parse(value); break;
                            case "MoveSpeed":
                                mobAttributes.MoveSpeed = float.Parse(value); break;
                            case "Flying":
                                mobAttributes.Flying = Convert.ToBoolean(int.Parse(value)); break;
                            case "HitSphere":
                                mobAttributes.HitSphere = float.Parse(value); break;
                            case "DeathAnimationDelay":
                                mobAttributes.DeathAnimationDelay = int.Parse(value); break;
                            case "DeathFadeDelay":
                                mobAttributes.DeathFadeDelay = int.Parse(value); break;
                            case "DeathFadeDuration":
                                mobAttributes.DeathFadeDuration = int.Parse(value); break;
                            case "HealthCanvasY":
                                mobAttributes.HealthCanvasY = float.Parse( value ); break;
                            case "Type":
                                if      ( mobAttributes.Types[0] == null ) mobAttributes.Types[0] = value;
                                else if ( mobAttributes.Types[1] == null ) mobAttributes.Types[1] = value;
                                break;
                        }
                    }

                    #endregion

                    MobIdToAttributes.Add(mobId, mobAttributes);
                    MobNameToAttributes.Add(mobName, mobAttributes);
                    MobFullNameToAttributes.Add("" + mobId + "_" + mobName, mobAttributes);

                    #region Set the mob type-to-name/id dictionary.

                    if (mobAttributes.Types[0] != null)
                    {
                        List<int> mobIdList;
                        List<string> mobNameList;
                        if (MobTypeToId.TryGetValue(mobAttributes.Types[0], out mobIdList))
                            mobIdList.Add(mobId);
                        else
                            MobTypeToId.Add(mobAttributes.Types[0], new List<int> {mobId});

                        if (MobTypeToName.TryGetValue(mobAttributes.Types[0], out mobNameList))
                            mobNameList.Add(mobName);
                        else
                            MobTypeToName.Add(mobAttributes.Types[0], new List<string> {mobName});
                        if (MobTypeToFullName.TryGetValue(mobAttributes.Types[0], out mobNameList))
                            mobNameList.Add( "" + mobId + "_" + mobName );
                        else
                            MobTypeToFullName.Add(mobAttributes.Types[0],
                                new List<string> { "" + mobId + "_" + mobName } );
                    }
                    if (mobAttributes.Types[1] != null)
                    {
                        List<int> mobIdList;
                        List<string> mobNameList;
                        if (MobTypeToId.TryGetValue(mobAttributes.Types[1], out mobIdList))
                            mobIdList.Add(mobId);
                        else
                            MobTypeToId.Add(mobAttributes.Types[1], new List<int> {mobId});

                        if (MobTypeToName.TryGetValue(mobAttributes.Types[1], out mobNameList))
                            mobNameList.Add(mobName);
                        else
                            MobTypeToName.Add(mobAttributes.Types[1], new List<string> {mobName});
                        if (MobTypeToFullName.TryGetValue(mobAttributes.Types[1], out mobNameList))
                            mobNameList.Add("" + mobId + "_" + mobName);
                        else
                            MobTypeToFullName.Add(mobAttributes.Types[1],
                                new List<string> {"" + mobId + "_" + mobName});
                    }

                        #endregion
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
            MobNameToId.Clear();
            MobNameToAttributes.Clear();
            MobIdToName.Clear();
            MobIdToAttributes.Clear();
            MobTypeToId.Clear();
            MobTypeToName.Clear();
            MobTypeToFullName.Clear();
            MobFullNameToAttributes.Clear();
        }
    }
}
