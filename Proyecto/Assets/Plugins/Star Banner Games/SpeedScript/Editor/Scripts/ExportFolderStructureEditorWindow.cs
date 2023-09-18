using System.IO;
using UnityEditor;
using UnityEngine;

namespace SBG.SpeedScript
{
    /// <summary>
    /// A Dialog Window to export a folder structure from the current project, using only subfolders of the selected Root Folder.
    /// </summary>
	public class ExportFolderStructureEditorWindow : EditorWindow
	{
        private string _rootFolder = "Assets";
        private string _structureName = "New Structure";

        private string[] _takenNames;


        /// <summary>
        /// Needs to be called after Window Creation to check for duplicate Filenames.
        /// </summary>
        public void SetStructureNames(string[] names)
        {
            _takenNames = names;
        }

        private void OnGUI()
        {
            //STRUCTURE NAME
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Structure Name");
            _structureName = EditorGUILayout.TextField(_structureName);
            EditorGUILayout.EndHorizontal();

            //ROOT FOLDER
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Root Folder");
            EditorGUILayout.LabelField($"{_rootFolder}", GUILayout.MaxWidth(180), GUILayout.MaxHeight(20));
            if (GUILayout.Button("...", GUILayout.MinWidth(50))) SetRootFolder();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //EXPORT
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export")) TryExport();
            //CANCEL
            if (GUILayout.Button("Cancel")) Close();

            EditorGUILayout.EndHorizontal();
        }

        private void TryExport()
        {
            if (NameAvailable())
            {
                ExportFolderStructure();
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Duplicate Name", $"The name \"{_structureName}\" is already used by a different Folder Structure!", "Ok");
            }
        }

        private void SetRootFolder()
        {
            _rootFolder = EditorUtility.OpenFolderPanel("Select Root Folder", Application.dataPath, "");

            if (_rootFolder.Contains(Application.dataPath))
            {
                _rootFolder = _rootFolder.Replace(Application.dataPath, string.Empty);
                _rootFolder = _rootFolder.Insert(0, "Assets");
            }
            else
            {
                _rootFolder = "Assets";
            }
        }

        private bool NameAvailable()
        {
            for (int i = 0; i < _takenNames.Length; i++)
            {
                if (_takenNames[i] == _structureName)
                {
                    return false;
                }
            }

            return true;
        }

        private void ExportFolderStructure()
        {
            string fileText = WriteFolderBranches(_rootFolder);

            string folderPath = GlobalPaths.UserStructuresFolderPath;

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{_structureName}.fs");

            File.WriteAllText(filePath, fileText);

            ScriptBuilder.Log($"\"{_structureName}.fs\" exported successfully!");
        }

        /// <summary>
        /// Recursive Function to return all Subfolders with Line Endings in-between.
        /// </summary>
        private string WriteFolderBranches(string path)
        {
            string[] children = AssetDatabase.GetSubFolders(path);

            if (children == null || children.Length == 0)
            {
                return path.Remove(0, _rootFolder.Length+1);
            }

            string fileText = "";

            for (int i = 0; i < children.Length; i++)
            {
                string childBranch = WriteFolderBranches(children[i]);

                if (!string.IsNullOrEmpty(childBranch))
                {
                    fileText += childBranch;

                    if (i + 1 < children.Length) fileText += "\r\n";
                }
            }

            return fileText;
        }
    }
}