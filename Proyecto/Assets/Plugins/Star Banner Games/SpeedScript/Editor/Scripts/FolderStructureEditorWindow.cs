using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SBG.SpeedScript
{
    public class FolderStructureEditorWindow : EditorWindow
	{
        #region CONSTANTS

        private const float EXPORT_STRUCTURE_POPUP_WIDTH = 400;
        private const float EXPORT_STRUCTURE_POPUP_HEIGHT = 120;

        private const string CREATE_FOLDERS_INFO = "You are about to setup the Folder Structure \"NAME.fs\" in your Asset Folder. Are you sure?";
        private const string DELETE_FOLDERS_WARNING = "You are about to delete all empty folders in your Asset Folder. Are you sure?";
        private const string DELETE_FS_WARNING = "You are about to delete the Folder Structure \"NAME.fs\". Are you sure?";
        private const string DELETE_DEFAULT_FS_INFO = "\"NAME.fs\" is a Default Folder Structure and can't be deleted.";

        #endregion

        #region VARIABLES

        private static bool _useProjectFolder = true;
        private static string _projectFolderName = "MyProject";
        private static int _structureTemplateIndex = 0;
        private static bool _relocateMatchingFolders = true;

        private static string[] _availableStructureNames;
        private static int _defaultStructureCount = 2;

        #endregion

        #region WINDOW MANAGEMENT

        [MenuItem(GlobalPaths.FOLDERSTRUCTURE_SETTINGS_MENUPATH)]
		public static void ShowWindow()
		{
			GetWindow<FolderStructureEditorWindow>("Folder Structure");
        }

        private void OnEnable()
        {
            _projectFolderName = PlayerSettings.productName;
        }

        private void OnFocus()
        {
            LoadStructureTemplates();

            if (_structureTemplateIndex >= _availableStructureNames.Length)
            {
                _structureTemplateIndex = 0;
            }
        }

        private void OnGUI()
        {
            //TOGGLE/SELECT ROOT FOLDER
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Project Root Folder");
            _useProjectFolder = EditorGUILayout.Toggle(_useProjectFolder, GUILayout.MaxWidth(15));
            if (_useProjectFolder) _projectFolderName = EditorGUILayout.TextField(_projectFolderName);
            EditorGUILayout.EndHorizontal();

            //SELECT/DELETE FOLDER STRUCTURE
            EditorGUILayout.BeginHorizontal();
            _structureTemplateIndex = EditorGUILayout.Popup("Folder Structure", _structureTemplateIndex, _availableStructureNames, GUILayout.MinWidth(170));
            if (GUILayout.Button("X", GUILayout.MaxWidth(30))) TryDeleteFolderStructure();
            EditorGUILayout.EndHorizontal();

            GUIContent sampleSceneToggleContent = new GUIContent("Relocate Folders", "If existing top-level folders match folders of the new folder structure, relocate them to the new structure.");
            _relocateMatchingFolders = EditorGUILayout.Toggle(sampleSceneToggleContent, _relocateMatchingFolders);

            EditorGUILayout.Space();

            //SETUP FOLDER STRUCTURE
            GUI.color = Color.green;
            if (GUILayout.Button("Setup", GUILayout.Height(30))) TrySetupFolderStructure();

            //CLEAN EMPTY FOLDERS
            GUI.color = Color.red;
            if (GUILayout.Button("Delete Empty Folders", GUILayout.Height(30))) TryCleanEmptyFolders();
            GUI.color = Color.white;
            EditorGUILayout.Space();

            //EXPORT STRUCTURE
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Project Structure...")) OpenStructureCreationWindow();
            //OPEN SOURCE
            if (GUILayout.Button("Open Source Folder")) TryOpenSourceFolder();
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region PRIVATE FUNCTIONS

        private void TrySetupFolderStructure()
        {
            string name = _availableStructureNames[_structureTemplateIndex];
            string message = CREATE_FOLDERS_INFO.Replace("NAME", name);

            if (EditorUtility.DisplayDialog("Setup Folder Structure", message, "Yes", "No"))
            {
                string[] subFolders = ReadFolderStructureFromFile();

                if (subFolders != null)
                {
                    CreateSubFolders(_projectFolderName, subFolders);
                }
            }
        }

        private bool TryRelocateFolder(string targetPath, string targetFolderName)
        {
            string[] assetFolders = AssetDatabase.GetSubFolders("Assets");
            if (assetFolders == null) return false;

            foreach (string folder in assetFolders)
            {
                if (folder == targetPath) continue;

                string folderName = folder.Replace("Assets/", string.Empty);

                if (folderName == targetFolderName)
                {
                    string result = AssetDatabase.MoveAsset(folder, targetPath);

                    if (string.IsNullOrEmpty(result))
                    {
                        ScriptBuilder.Log($"Relocated Folder \"{folder}\" to \"{targetPath}\"");
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning(result);
                    }
                }
            }

            return false;
        }

        private void TryCleanEmptyFolders()
        {
            if (EditorUtility.DisplayDialog("Delete Empty Folders", DELETE_FOLDERS_WARNING, "Yes", "No"))
            {
                DeleteEmptySubfolders(Application.dataPath);
                ScriptBuilder.Log("All empty folders deleted!");
                AssetDatabase.Refresh();
            }
        }

        private static void TryOpenSourceFolder()
        {
            string path = GlobalPaths.UserStructuresFolderPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
                
            System.Diagnostics.Process.Start(path);
        }

        private void TryDeleteFolderStructure()
        {
            string fsName = _availableStructureNames[_structureTemplateIndex];

            if (_structureTemplateIndex >= _defaultStructureCount)
            {
                string warning = DELETE_FS_WARNING.Replace("NAME", fsName);

                if (EditorUtility.DisplayDialog($"Delete {fsName}", warning, "Yes", "No"))
                {
                    DeleteSelectedFolderStructure(_availableStructureNames[_structureTemplateIndex]);
                }
            }
            else
            {
                string warning = DELETE_DEFAULT_FS_INFO.Replace("NAME", fsName);
                EditorUtility.DisplayDialog($"Delete {fsName}", warning, "Ok");
            }
        }

        private void OpenStructureCreationWindow()
        {
            Rect popupRect = new Rect();

            popupRect.width = EXPORT_STRUCTURE_POPUP_WIDTH;
            popupRect.height = EXPORT_STRUCTURE_POPUP_HEIGHT;

            popupRect.x = (Screen.currentResolution.width / 2) - EXPORT_STRUCTURE_POPUP_WIDTH;
            popupRect.y = (Screen.currentResolution.height / 2) - EXPORT_STRUCTURE_POPUP_HEIGHT;

            var window = GetWindowWithRect<ExportFolderStructureEditorWindow>(popupRect, true, "Export Project Folder Structure");

            window.SetStructureNames(_availableStructureNames);
        }

        private void DeleteSelectedFolderStructure(string displayName)
        {
            string fileName = $"{displayName}.fs";

            string path = Path.Combine(GlobalPaths.UserStructuresFolderPath, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                ScriptBuilder.Log($"\"{fileName}\" has been deleted.");

                _structureTemplateIndex--;
                if (_structureTemplateIndex < 0) _structureTemplateIndex = 0;

                LoadStructureTemplates();
            }
            else
            {
                ScriptBuilder.LogError($"\"{fileName}\" does not exist.");
            }
        }

        private void LoadStructureTemplates()
        {
            List<string> structures = new List<string>();

            //LOAD DEFAULT STRUCTURES
            string defaultStructurePath = GlobalPaths.StructureFolderPath;

            if (Directory.Exists(defaultStructurePath))
            {
                string[] files = Directory.GetFiles(defaultStructurePath, "*.fs");

                if (files != null)
                {
                    _defaultStructureCount = files.Length;
                    structures.AddRange(files);
                }
                else
                {
                    _defaultStructureCount = 0;
                }
            }


            //LOAD USER STRUCTURES
            string userStructurePath = GlobalPaths.UserStructuresFolderPath;

            if (Directory.Exists(userStructurePath))
            {
                string[] files = Directory.GetFiles(userStructurePath, "*.fs");

                if (files != null)
                {
                    structures.AddRange(files);
                }
            }


            //EXTRACT ALL STRUCTURE NAMES
            string[] structureNames = new string[structures.Count];

            for (int i = 0; i < structures.Count; i++)
            {
                string fileName = GlobalPaths.GetFolderNameFromPath(structures[i]);

                structureNames[i] = fileName.Replace(".fs", string.Empty);
            }

            _availableStructureNames = structureNames;
        }

        private string[] ReadFolderStructureFromFile()
        {
            if (_availableStructureNames == null || _availableStructureNames.Length <= 0)
            {
                ScriptBuilder.LogError("Folder Structure File not found!");
                return null;
            }

            string path;

            if (_structureTemplateIndex < _defaultStructureCount)
            {
                path = Path.Combine(GlobalPaths.StructureFolderPath, _availableStructureNames[_structureTemplateIndex]);
            }
            else
            {
                path = Path.Combine(GlobalPaths.UserStructuresFolderPath, _availableStructureNames[_structureTemplateIndex]);
            }

            path += ".fs";

            if (File.Exists(path))
            {
                return File.ReadAllLines(path);
            }
            else
            {
                ScriptBuilder.LogError("Folder Structure File not found!");
                return null;
            }
        }

        private void CreateSubFolders(string projectFolder, string[] folderNames)
        {
            bool hasProjectFolder = true;

            if (!_useProjectFolder || string.IsNullOrEmpty(projectFolder))
            {
                hasProjectFolder = false;
            }

            for (int i = 0; i < folderNames.Length; i++)
            {
                if (hasProjectFolder)
                {
                    folderNames[i] = $"{projectFolder}/{folderNames[i]}";
                }

                string[] splitName = folderNames[i].Split('/');

                string rootPath;
                string path = "Assets";

                for (int j = 0; j < splitName.Length; j++)
                {
                    rootPath = path;
                    path = $"{path}/{splitName[j]}";

                    if (AssetDatabase.IsValidFolder(path) == false)
                    {
                        bool skip = false;

                        if (_relocateMatchingFolders) skip = TryRelocateFolder(path, splitName[j]);

                        if (!skip) AssetDatabase.CreateFolder(rootPath, splitName[j]);
                    }
                }
            }

            ScriptBuilder.Log("Folder Structure has been setup successfully!");
        }


        /// <summary>
        /// Recursively deletes all empty subfolders in this directory
        /// </summary>
        private void DeleteEmptySubfolders(string rootPath)
        {
            if (!Directory.Exists(rootPath)) return;

            string[] subFolders = Directory.GetDirectories(rootPath);

            if (subFolders != null)
            {
                for (int i = 0; i < subFolders.Length; i++)
                {
                    DeleteEmptySubfolders(subFolders[i]);
                }

                subFolders = Directory.GetDirectories(rootPath);
            }

            string[] subFiles = Directory.GetFiles(rootPath);

            if ((subFolders == null || subFolders.Length == 0) &&
                (subFiles == null || subFiles.Length == 0))
            {
                Directory.Delete(rootPath);
                File.Delete($"{rootPath}.meta");
            }
        }

        #endregion
    }
}