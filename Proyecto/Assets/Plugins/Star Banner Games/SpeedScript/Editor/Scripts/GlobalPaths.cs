using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace SBG.SpeedScript
{
    public static class GlobalPaths
    {
        #region CONSTANTS

        public const string FOLDERSTRUCTURE_SETTINGS_MENUPATH = "Window/Star Banner Games/Speed Script/Folder Structure";
        public const string TEMPLATE_SETTINGS_MENUPATH = "Window/Star Banner Games/Speed Script/Template Settings";

        public const string TEMPLATE_MENUPATH = "Assets/Create/C# (Speed Script)/";
        public const string TEMPLATE_EDITOR_MENUPATH = "Assets/Create/C# (Speed Script)/Editor/";

        private static readonly char[] SLASHES = {'/', '\\'};

        #endregion

        #region CACHE VARIABLES

        //Cache to avoid iterating through folder structure unless the path breaks.
        private static string _fullProjectPath;
        private static string _relativeProjectPath;

        #endregion

        #region PATH PROPERTIES

        //BASE PATHS
        public static string ProjectFolderPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_fullProjectPath) && Directory.Exists(_fullProjectPath))
                {
                    return _fullProjectPath;
                }
                else
                {
                    return GetProjectFolder(false);
                }
            }
        }
        public static string RelativeProjectFolderPath
        {
            get
            {
                if (!string.IsNullOrEmpty(_relativeProjectPath) &&
                    Directory.Exists(_fullProjectPath))
                {
                    return _relativeProjectPath;
                }
                else
                {
                    return GetProjectFolder(true);
                }
            }
        }
        public static string UserTemplatesFolderPath
        {
            get
            {
                string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                return Path.Combine(docsPath, "Star Banner Games", "SpeedScript", "UserData", "Templates");
            }
        }
        public static string UserStructuresFolderPath
        {
            get
            {
                string docsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                return Path.Combine(docsPath, "Star Banner Games", "SpeedScript", "UserData", "Folder Structures");
            }
        }

        //SUB PATHS
        public static string ScriptsFolderPath => Path.Combine(ProjectFolderPath, "Editor", "Scripts");
        public static string RelativeScriptsFolderPath => Path.Combine(RelativeProjectFolderPath, "Editor", "Scripts");
        public static string DataFolderPath => Path.Combine(ProjectFolderPath, "Editor", "Data");
        public static string RelativeDataFolderPath => Path.Combine(RelativeProjectFolderPath, "Editor", "Data");
        public static string InternalDataFolderPath => Path.Combine(DataFolderPath, "Internal");
        public static string TemplateFolderPath => Path.Combine(DataFolderPath, "Templates");
        public static string RelativeTemplateFolderPath => Path.Combine(RelativeDataFolderPath, "Templates");
        public static string StructureFolderPath => Path.Combine(DataFolderPath, "Folder Structures");
        public static string ConfigPath => Path.Combine(InternalDataFolderPath, "Config.json");
        public static string ActiveMenuItemsPath => Path.Combine(InternalDataFolderPath, "ActiveItems.txt");

        #endregion

        #region HELPER FUNCTIONS

        public static string GetFolderNameFromPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            int nameIndex = path.LastIndexOfAny(SLASHES) + 1;
            return path.Substring(nameIndex);
        }

        private static string GetProjectFolder(bool relativeToAssets)
        {
            string sbgPath = GetFolderPathRecursive(Application.dataPath, "SpeedScript");

            _fullProjectPath = sbgPath;

            if (relativeToAssets)
            {
                sbgPath = sbgPath.Replace(Application.dataPath, "Assets");
                _relativeProjectPath = sbgPath;
            }

            return sbgPath;
        }

        private static string GetFolderPathRecursive(string rootPath, string targetName)
        {
            if (!Directory.Exists(rootPath)) return null;

            string[] subPaths = Directory.GetDirectories(rootPath);

            if (subPaths == null) return null;

            for (int i = 0; i < subPaths.Length; i++)
            {
                if (GetFolderNameFromPath(subPaths[i]) == targetName)
                {
                    return subPaths[i];
                }

                string folderPath = GetFolderPathRecursive(subPaths[i], targetName);

                if (GetFolderNameFromPath(folderPath) == targetName)
                {
                    return folderPath;
                }
            }

            return null;
        }

        #endregion
    }
}