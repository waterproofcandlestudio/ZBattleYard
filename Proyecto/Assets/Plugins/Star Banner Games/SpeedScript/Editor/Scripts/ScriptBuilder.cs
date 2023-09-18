using System.IO;
using UnityEditor;
using UnityEngine;

namespace SBG.SpeedScript
{
    [InitializeOnLoad]
    public sealed class ScriptBuilder : UnityEditor.AssetModificationProcessor
    {
        #region CONSTANTS

        public const int TEMPLATE_NORMAL_MENU_PRIORITY = 81; //Picked so it appears right before the default "C# Scripts" menu item.
        public const int TEMPLATE_CUSTOM_MENU_PRIORITY = 100;
        public const int TEMPLATE_EDITOR_MENU_PRIORITY = 120;
        public const int TEMPLATE_CUSTOMEDITOR_MENU_PRIORITY = 140;

        #endregion

        private static SpeedScriptConfig _config;


        static ScriptBuilder()
        {
            if (SessionState.GetBool("SpeedScriptLoaded", false) == false)
            {
                SessionState.SetBool("SpeedScriptLoaded", true);
                TemplateSettingsEditorWindow.RefreshCustomTemplates();
            }
        }


        #region DEBUG MESSAGES

        ///Not pretty to put these in here, but it doesn't feel worth its own class

        public static void Log(string text)
        {
            Debug.Log($"SPEED SCRIPT: {text}");
        }

        public static void LogWarning(string text)
        {
            Debug.LogWarning($"SPEED SCRIPT: {text}");
        }

        public static void LogError(string text)
        {
            Debug.LogError($"SPEED SCRIPT: {text}");
        }

        #endregion

        #region SCRIPT CREATION

        /// <summary>
        /// Looks for a Template in the Data Folder and creates a script from it.
        /// </summary>
        public static void CreateScriptFromTemplate(string templateName, string defaultScriptName)
        {
            string templatePath = Path.Combine(GlobalPaths.RelativeTemplateFolderPath, $"{templateName}.cs.txt");

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"{defaultScriptName}.cs");
        }

        /// <summary>
        /// Looks for a Template in the external UserData Folder and creates a script from it.
        /// </summary>
        public static void CreateScriptFromCustomTemplate(string templateName, string defaultScriptName)
        {
            string templatePath = Path.Combine(GlobalPaths.UserTemplatesFolderPath, $"{templateName}.cs.txt");

            try
            {
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"{defaultScriptName}.cs");
            }
            catch
            {
                ScriptBuilder.LogWarning("Template not found! Reimporting Custom Templates...");
                TemplateSettingsEditorWindow.RefreshCustomTemplates();
            }
        }

        public static void OnWillCreateAsset(string path)
        {
            ///THIS FUNCTION IS CALLED FROM "UnityEditor.AssetModificationProcessor"
            ///The Path passed to the function starts with "Assets/..."
            ///and leads to the .meta file of the created asset.
            ///We need to:
            ///1. REMOVE the .meta
            ///2. Check if it is a .cs file (meta files go "name.ending.meta" => "scriptname.cs.meta")
            ///3. Tinker with the file
            ///4. Refresh the Assets so Unity displays the changes

            //1
            path = path.Replace(".meta", string.Empty);

            //2
            if (!path.EndsWith(".cs"))
            {
                return;
            }

            ///COMMENT FOR BACKUP:
            ///I thought it was necessary to convert the relative file path to the system path.
            ///This would mess up script creation in package folders (outside "Assets").
            ///Turns out its not necessary. I just wanna keep the code here in case the update breaks.
            ///string pathNoAssets = path;
            ///string assetString = "Assets/";
            ///if (path.StartsWith(assetString)) pathNoAssets = path.Remove(0, assetString.Length);
            ///string systemPath = $"{Application.dataPath}/{pathNoAssets}";

            //3
            ReplaceKeywords(path);

            //4
            AssetDatabase.Refresh();
        }

        private static void ReplaceKeywords(string path)
        {
            _config = LoadConfig();

            string fileData = File.ReadAllText(path);

            if (fileData.Contains("#NAMESPACE#"))
            {
                string fullNamespace = GetNamespace(path);
                fileData = fileData.Replace("#NAMESPACE#", fullNamespace);
            }

            if (fileData.Contains("#SCRIPTNAME!EDITOR#"))
            {
                string fileNameNoEditor = GetFilenameNoEditor(path);
                fileData = fileData.Replace("#SCRIPTNAME!EDITOR#", fileNameNoEditor);
            }

            File.WriteAllText(path, fileData);
        }

        #endregion

        #region LOAD DATA

        public static SpeedScriptConfig LoadConfig()
        {
            SpeedScriptConfig config = new SpeedScriptConfig();
            string path = GlobalPaths.ConfigPath;

            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);

                config = JsonUtility.FromJson<SpeedScriptConfig>(content);

                if (config.UsePlayerSettings)
                {
                    config.DefaultNamespace = $"{PlayerSettings.companyName}.{PlayerSettings.productName}";
                    config.DefaultNamespace = config.DefaultNamespace.Replace(" ", string.Empty);
                }
            }

            return config;
        }

        public static void SaveConfig(SpeedScriptConfig config)
        {
            string path = GlobalPaths.ConfigPath;

            string jsonContent = JsonUtility.ToJson(config, true);

            try
            {
                File.WriteAllText(path, jsonContent);
            }
            catch
            {
                ScriptBuilder.LogError("Could not apply changes!");
            }
        }

        private static string GetNamespace(string projectPath)
        {
            string fullNamespace;

            if (_config.UseFolderNames)
            {
                string slashIgnore = $"/{_config.IgnoreFolder}/";

                //Limit namespace Hierarchy to any folder after "Scripts"
                int ignoreIndex = projectPath.IndexOf(slashIgnore);
                if (ignoreIndex > -1)
                {
                    fullNamespace = projectPath.Substring(ignoreIndex + slashIgnore.Length);
                }
                else
                {
                    string assetString = "Assets/";
                    ignoreIndex = projectPath.IndexOf(assetString);
                    fullNamespace = projectPath.Substring(ignoreIndex + assetString.Length);
                }

                //Remove Filename to only get directory path
                if (fullNamespace.Contains("/"))
                {
                    fullNamespace = fullNamespace.Remove(fullNamespace.LastIndexOf('/'));
                    //Replace Slashes with dots for Namespace Syntax
                    fullNamespace = fullNamespace.Replace('/', '.');
                    //Add Default Namespace to start of Folder Namespace
                    fullNamespace = $"{_config.DefaultNamespace}.{fullNamespace}";
                }
                else
                {
                    fullNamespace = _config.DefaultNamespace;
                }

                if (string.IsNullOrEmpty(fullNamespace))
                {
                    fullNamespace = _config.DefaultNamespace;
                }
            }
            else
            {
                fullNamespace = _config.DefaultNamespace;
            }

            fullNamespace = fullNamespace.Replace(" ", string.Empty);

            return fullNamespace;
        }

        private static string GetFilenameNoEditor(string projectPath)
        {
            string fileName = GlobalPaths.GetFolderNameFromPath(projectPath);

            int cutOffIndex = fileName.LastIndexOf("Editor.cs");
            if (cutOffIndex > -1)
            {
                return fileName.Remove(cutOffIndex);
            }

            cutOffIndex = fileName.LastIndexOf("Inspector.cs");
            if (cutOffIndex > -1)
            {
                return fileName.Remove(cutOffIndex);
            }

            cutOffIndex = fileName.LastIndexOf(".cs");
            fileName = "YourClass";

            return fileName;
        }

        #endregion
    }
}