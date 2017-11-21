using System;
using System.Collections.Generic;
using Assets.Scripts.FileUtilities;
using UnityEngine;

namespace Assets.Scripts.ElementScripts
{
    internal class ElementDictionary
    {
        public const string ElementDir = "ElementTypes";
        private const string AttributesFile = "Attributes";

        #region Singleton

        private static readonly ElementDictionary I = new ElementDictionary();
        static ElementDictionary() { }
        private ElementDictionary() { }
        public static ElementDictionary Instance { get { return I; } }

        #endregion

        public bool Initialized { private set; get; }

        public readonly Dictionary<string, int>                 ElementNameToId             = new Dictionary<string, int>();
        public readonly Dictionary<string, ElementAttributes>   ElementNameToAttributes     = new Dictionary<string, ElementAttributes>();
        public readonly Dictionary<int, string>                 ElementIdToName             = new Dictionary<int, string>();
        public readonly Dictionary<int, ElementAttributes>      ElementIdToAttributes       = new Dictionary<int, ElementAttributes>();
        public readonly Dictionary< string, ElementAttributes > ElementFullNameToAttributes = new Dictionary<string, ElementAttributes>();

        public bool Initialize()
        {
            if (Initialized)
            {
                Debug.LogWarning("Element Dictionary is already initialized!");
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

                #region populate folders with string entries of all folders in Resources/Elements

                var tmpDictionary = new Dictionary<string, bool>();
                foreach (var text in manifestText)
                {
                    var textSplit = text.Split('\\', '/');
                    if (textSplit.Length == 1) continue;
                    if (textSplit[0] != ElementDir) continue;
                    if (tmpDictionary.ContainsKey(textSplit[1])) continue;
                    folders.Add(textSplit[1]);
                    tmpDictionary.Add(textSplit[1], true);
                }
                tmpDictionary.Clear();

                #endregion

                foreach (var folder in folders)
                {
                    #region Obtain the text asset and find the element name and Id.

                    var attributeList = Resources.Load(ElementDir + "/" + folder + "/" + AttributesFile) as TextAsset;
                    if (!attributeList)
                    {
                        Debug.Log("Could not load the attribute file of " + folder);
                        return false;
                    }
                    var elementAttributes = new ElementAttributes();
                    var elementId = int.Parse(folder.Split('_')[0]);
                    var elementName = folder.Split('_')[1];

                    #endregion

                    ElementNameToId.Add(elementName, elementId);
                    ElementIdToName.Add(elementId, elementName);

                    #region Get parse the element's attributes and store into an attribute object and referenced by a dictionary.

                    var attributeStrings = attributeList.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    foreach (var attributeString in attributeStrings)
                    {
                        var tmp = attributeString.Trim().Replace(" ", "");
                        var tmp2 = tmp.Split(':');
                        if (tmp2.Length == 1) continue;
                        var attribute = tmp2[0];
                        var value = tmp2[1];
                        switch (attribute)
                        {
                            case "Strength":
                                elementAttributes.Strength.Add(value); break;
                            case "Weakness":
                                elementAttributes.Weakness.Add(value); break;
                            case "Ineffective":
                                elementAttributes.Ineffective.Add(value); break;
                        }
                    }

                    #endregion

                    ElementIdToAttributes.Add(elementId, elementAttributes);
                    ElementNameToAttributes.Add(elementName, elementAttributes);
                    ElementFullNameToAttributes.Add( "" + elementId + "_" + elementName, elementAttributes );
                }
            }
            catch (Exception e)
            {
                Debug.LogError( e.Message );
                Debug.LogError( e.StackTrace );
                _clearDictionaries();
                return false;
            }
            return Initialized = true;
        }

        private void _clearDictionaries()
        {
            ElementNameToId.Clear();
            ElementNameToAttributes.Clear();
            ElementIdToName.Clear();
            ElementIdToAttributes.Clear();
            ElementFullNameToAttributes.Clear();
        }
    }
}
