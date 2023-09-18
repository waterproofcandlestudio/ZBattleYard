using UnityEditor;
using UnityEngine;

namespace SBG.SpeedScript
{
    public class CreateTemplateEditorWindow : EditorWindow
	{
        private string _templateName = "CustomScript";
        private bool _isEditorTemplate = false;

        private void OnGUI()
        {
            EditorGUILayout.Space();

            _templateName = EditorGUILayout.TextField("Template Name", _templateName);
            _isEditorTemplate = EditorGUILayout.Toggle("Used for Editor Scripts", _isEditorTemplate);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Create"))
            {
                var scriptBuilderWindow = TemplateSettingsEditorWindow.ShowWindow();

                scriptBuilderWindow.AddTemplate(_templateName, _isEditorTemplate);

                ExitWindow();
            }

            if (GUILayout.Button("Cancel"))
            {
                ExitWindow();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ExitWindow()
        {
            Close();
            GUIUtility.ExitGUI();
        }
    }


}