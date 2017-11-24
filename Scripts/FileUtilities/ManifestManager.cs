using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.FileUtilities
{
    public class ManifestManager
    {
        #region Singleton

        static ManifestManager() { }
        private ManifestManager() { }

        #endregion

        private static readonly string Dir = Application.dataPath + "/Resources/";
        private const string ManifestName = "Manifest";
        private const string ManifestExt = ".txt";

        public static bool CreateManifest()
        {
            string[] directories;
            var finalDir = new List<string>();
            try
            {
                directories = Directory.GetDirectories( Dir, "*", SearchOption.AllDirectories );
            }
            catch (Exception e)
            {
                Debug.Log( e.Message );
                return false;
            }

            foreach (var dir in directories)
            {
                var resourceDir = dir.Remove( 0, Dir.Length );
                var fileName = resourceDir.Split( '/', '\\' ).Last();
                var fileTag = fileName.Split('.').Last();
                if ( fileTag == "meta" ) continue;
                finalDir.Add( resourceDir );
            }

            // Check if file already exists. If yes, delete it. 
            if ( File.Exists( Dir + ManifestName + ManifestExt ) )
            {
                File.Delete( Dir + ManifestName + ManifestExt );
            }
            var writer = File.CreateText(Dir + ManifestName + ManifestExt );
            foreach (var dir in finalDir)
            {
                writer.Write(dir);
                writer.WriteLine();
            }
            writer.Flush();
            writer.Close();
            return true;
        }

        public static string[] OpenManifest()
        {
            var text = Resources.Load( ManifestName ) as TextAsset;
            if ( !text && !CreateManifest() )
            {
                return null;
            }
            text = Resources.Load( ManifestName ) as TextAsset;
            if (!text)
            {
                return null;
            }
            var ret = text.text.Split( new[]{ "\r\n", "\r", "\n" }, StringSplitOptions.None );
            return ret;
        }
    }
}
