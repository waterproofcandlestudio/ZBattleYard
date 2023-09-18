using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SBG.SpeedScript
{
    public class TemplateSettingsEditorWindow : EditorWindow
	{
        #region CONSTANTS

        private const float WINDOW_WIDTH = 550;
		private const float WINDOW_HEIGHT = 350;

		private const float CREATE_TEMPLATE_POPUP_WIDTH = 350;
		private const float CREATE_TEMPLATE_POPUP_HEIGHT = 80;

		private const string DELETE_TEMPLATE_WARNING = "You are about to delete the script template \"NAME\". Are you sure?";

		#endregion

		#region VARIABLES

		private SpeedScriptConfig _config = new SpeedScriptConfig();

		private bool _namespaceFoldout = false;
		private bool _templateFoldout = false;
		private Vector2 _templateScrollPos = Vector2.zero;
		private GUIStyle[] _templateListStyles = new GUIStyle[2];

		private static string[] _userTemplatePaths;
		private static string[] _userTemplateNames;
		private static bool[] _userTemplatesIsEditor;

        #endregion

        #region WINDOW MANAGEMENT

        [MenuItem(GlobalPaths.TEMPLATE_SETTINGS_MENUPATH)]
		public static TemplateSettingsEditorWindow ShowWindow()
        {
            var window = GetWindow<TemplateSettingsEditorWindow>("Template Settings");

			window.ResetTemplateListStyles();

			Rect windowRect = new Rect();

            windowRect.size = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
            windowRect.x = (Screen.currentResolution.width / 2) - WINDOW_WIDTH;
            windowRect.y = (Screen.currentResolution.height / 2) - WINDOW_HEIGHT;

            window.position = windowRect;

            return window;
        }

		private void OpenTemplateCreationWindow()
		{
			Rect popupRect = new Rect();

			popupRect.width = CREATE_TEMPLATE_POPUP_WIDTH;
			popupRect.height = CREATE_TEMPLATE_POPUP_HEIGHT;

			popupRect.x = (Screen.currentResolution.width / 2) - CREATE_TEMPLATE_POPUP_WIDTH;
			popupRect.y = (Screen.currentResolution.height / 2) - CREATE_TEMPLATE_POPUP_HEIGHT;

			GetWindowWithRect<CreateTemplateEditorWindow>(popupRect, true, "Create Script Template");
		}

		private void OnFocus()
		{
			LoadEditorPrefs();
			ResetTemplateListStyles();
			_config = ScriptBuilder.LoadConfig();
			LoadUserTemplates();
		}

		private void OnLostFocus()
		{
			SaveEditorPrefs();
			ScriptBuilder.SaveConfig(_config);
		}

		private void OnGUI()
		{
			//NAMESPACE SETTINGS
			EditorGUILayout.Space();
			_namespaceFoldout = EditorGUILayout.Foldout(_namespaceFoldout, "Namespace Settings", true, EditorStyles.foldoutHeader);
			if (_namespaceFoldout)
			{
				string locationPreview = "Assets/.../";
				string namespacePreview = _config.DefaultNamespace;

				if (_config.UseFolderNames)
				{
					locationPreview += $"{_config.IgnoreFolder}/Folder/SubFolder/";

					if (_config.LastFolderOnly)
                    {
						namespacePreview += ".Folder";
					}

					namespacePreview += ".SubFolder";
				}

				locationPreview += "YourScript.cs";

				EditorGUILayout.BeginVertical(EditorStyles.helpBox);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Example Location:");
				EditorGUILayout.LabelField(locationPreview, EditorStyles.miniLabel);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel("Example Namespace:");
				EditorGUILayout.LabelField(namespacePreview, EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();

				bool prevPlayerSettings = _config.UsePlayerSettings;
				_config.UsePlayerSettings = EditorGUILayout.Toggle("Use Player Settings", _config.UsePlayerSettings);

				if (_config.UsePlayerSettings)
                {
					if (prevPlayerSettings != _config.UsePlayerSettings)
                    {
						_config.DefaultNamespace = $"{PlayerSettings.companyName}.{PlayerSettings.productName}";
						RemoveWhitespacesFromNamespaces();
					}

					EditorGUILayout.LabelField("Default Namespace", _config.DefaultNamespace);
				}
                else
                {
					string newNamespace = EditorGUILayout.TextField("Default Namespace", _config.DefaultNamespace);

					if (newNamespace != _config.DefaultNamespace)
                    {
						_config.DefaultNamespace = newNamespace;
						RemoveWhitespacesFromNamespaces();
					}
				}

				_config.UseFolderNames = EditorGUILayout.Toggle("Use Folder Names", _config.UseFolderNames);

				if (_config.UseFolderNames)
				{
					_config.LastFolderOnly = EditorGUILayout.Toggle("Last Folder Only", _config.LastFolderOnly);
					_config.IgnoreFolder = EditorGUILayout.TextField("Script Folder Name", _config.IgnoreFolder);
				}
			}

			//TEMPLATE CREATOR
			EditorGUILayout.Space();
			_templateFoldout = EditorGUILayout.Foldout(_templateFoldout, "Template Creator", true, EditorStyles.foldoutHeader);
			if (_templateFoldout)
			{
				EditorGUILayout.Space();
				_templateScrollPos = EditorGUILayout.BeginScrollView(_templateScrollPos);
				
				//LIST OF TEMPLATES
				int buttonStyleIndex = 0;
				if (_userTemplatePaths != null && _userTemplatePaths.Length > 0)
				{
					for (int i = 0; i < _userTemplatePaths.Length; i++)
					{
						EditorGUILayout.BeginHorizontal(_templateListStyles[i % 2]);
						EditorGUILayout.LabelField(_userTemplateNames[i]);
						GUILayout.FlexibleSpace();

						//DISPLAY IF EDITOR
						if (_userTemplatesIsEditor[i])
						{
							EditorGUILayout.LabelField("[Editor]", EditorStyles.boldLabel);
							GUILayout.FlexibleSpace();
						}
						//EDIT TEMPLATE BUTTON
						if (GUILayout.Button("Edit", GUILayout.MaxWidth(40), GUILayout.MinWidth(40))) OpenTemplateFile(_userTemplatePaths[i]);
						//DELETE TEMPLATE BUTTON
						if (GUILayout.Button("Delete", GUILayout.MaxWidth(50), GUILayout.MinWidth(50))) TryDeleteTemplate(i);
						EditorGUILayout.EndHorizontal();
					}

					buttonStyleIndex = _userTemplatePaths.Length % 2;
				}
				else
				{
					EditorGUILayout.BeginHorizontal(_templateListStyles[buttonStyleIndex]);
					EditorGUILayout.LabelField("No Custom Templates available...");
					EditorGUILayout.EndHorizontal();
					buttonStyleIndex = 1;
				}

				//REIMPORT TEMPLATE BUTTON
				EditorGUILayout.BeginHorizontal(_templateListStyles[buttonStyleIndex]);
				if (GUILayout.Button("Reimport")) RefreshCustomTemplates();
				//ADD TEMPLATE BUTTON
				if (GUILayout.Button("Add New")) OpenTemplateCreationWindow();
				EditorGUILayout.EndHorizontal();

				//OPEN SOURCE BUTTON
				buttonStyleIndex = 1 - buttonStyleIndex;
				EditorGUILayout.BeginHorizontal(_templateListStyles[buttonStyleIndex]);
				if (GUILayout.Button("Open Template Folder")) TryOpenSourceFolder();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndScrollView();
			}
		}

		private void ResetTemplateListStyles()
		{
			_templateListStyles = new GUIStyle[2];

			if (EditorGUIUtility.isProSkin)
			{
				_templateListStyles[0] = CreateBackgroudStyle(new Color(0.25f, 0.25f, 0.25f));
				_templateListStyles[1] = CreateBackgroudStyle(new Color(0.3f, 0.3f, 0.3f));
			}
			else
			{
				_templateListStyles[0] = CreateBackgroudStyle(new Color(0.6f, 0.6f, 0.6f));
				_templateListStyles[1] = CreateBackgroudStyle(new Color(0.7f, 0.7f, 0.7f));
			}
		}

		private static GUIStyle CreateBackgroudStyle(Color col)
		{
			Texture2D texture = new Texture2D(1, 1);
			texture.SetPixel(0, 0, col);
			texture.Apply();

			GUIStyle style = new GUIStyle();
			style.normal.background = texture;
			style.margin = new RectOffset(15, 15, 0, 0);
			return style;
		}

        #endregion

        #region TEMPLATE FUNCTIONS

			#region TEMPLATE CREATION

        public void AddTemplate(string name, bool editorTemplate)
        {
            name = name.Replace(" ", string.Empty);

			string displayName = SetDisplayName(name);

            if (TemplateNameIsTaken(name)) return;

            CreateAndOpenTemplateFile(name, editorTemplate);

			RefreshCustomTemplates();
        }

        private bool TemplateNameIsTaken(string name)
		{
			if (_userTemplateNames != null)
			{
				for (int i = 0; i < _userTemplateNames.Length; i++)
				{
					if (_userTemplateNames[i] == name)
					{
						EditorUtility.DisplayDialog("Duplicate Name", $"Template Name {name} already exists!", "Ok");
						return true;
					}
				}
			}

			return false;
		}

		private static void AddTemplateContextMenu(string name, string displayName, bool editorTemplate, bool recompileScript=true)
		{
			string contextMenuFileName;
			string templateRegionName;

			if (!editorTemplate)
			{
				contextMenuFileName = "ContextMenuItem.txt";
				templateRegionName = "TEMPLATES";
			}
			else
			{
				contextMenuFileName = "ContextMenuItem_Editor.txt";
				templateRegionName = "EDITOR TEMPLATES";
			}

			string scriptPath = Path.Combine(GlobalPaths.ScriptsFolderPath, "CustomContextMenuItems.cs");
			string contentPath = Path.Combine(GlobalPaths.InternalDataFolderPath, contextMenuFileName);

			if (File.Exists(scriptPath) && File.Exists(contentPath))
			{
				string targetCode = File.ReadAllText(scriptPath);
				string menuItemContent = File.ReadAllText(contentPath);

				menuItemContent = menuItemContent.Replace("#DPNAME#", displayName);
				menuItemContent = menuItemContent.Replace("#NAME#", name);

				targetCode = targetCode.Replace($"#region {templateRegionName}", menuItemContent);

				File.WriteAllText(scriptPath, targetCode);

				if (recompileScript)
                {
					AssetDatabase.SaveAssets();
					AssetDatabase.ImportAsset(Path.Combine(GlobalPaths.RelativeScriptsFolderPath, "CustomContextMenuItems.cs"));
				}

				ScriptBuilder.Log("Template added!");
			}
			else
			{
				ScriptBuilder.LogError("Important Source Files not found! Consider reimporting SpeedScript.");
			}
		}

		private void CreateAndOpenTemplateFile(string name, bool editorTemplate)
		{
			string userDataPath = GlobalPaths.UserTemplatesFolderPath;

			if (!Directory.Exists(userDataPath))
			{
				Directory.CreateDirectory(userDataPath);
			}

			string editorIdentifier = "";

			if (editorTemplate)
			{
				editorIdentifier = "Editor_";
			}

			string contentPath = Path.Combine(GlobalPaths.InternalDataFolderPath, "NewTemplateContent.txt");
			string filePath = Path.Combine(userDataPath, $"Template_{editorIdentifier}{name}.cs.txt");

			if (File.Exists(filePath) || !File.Exists(contentPath)) return;

			string defaultContent = File.ReadAllText(contentPath);
			File.WriteAllText(filePath, defaultContent);

			System.Diagnostics.Process.Start(filePath);
		}

			#endregion

			#region TEMPLATE DELETION

		private void TryDeleteTemplate(int i)
		{
			string warning = DELETE_TEMPLATE_WARNING.Replace("NAME", _userTemplateNames[i]);

			if (EditorUtility.DisplayDialog("Delete Template", warning, "Yes", "No"))
			{
				RemoveTemplate(i);
			}
		}

		private void RemoveTemplate(int index)
		{
			DeleteTemplateFile(_userTemplatePaths[index]);
			RefreshCustomTemplates();
		}

		private static void RemoveTemplateContextMenu(string name, bool recompileScript=true)
		{
			string scriptPath = Path.Combine(GlobalPaths.ScriptsFolderPath, "CustomContextMenuItems.cs");

			if (File.Exists(scriptPath))
			{
				string targetCode = File.ReadAllText(scriptPath);

				int startIndex = targetCode.IndexOf($"\r\n\t\t//MENU: {name}");
				string endingKey = $"//END: {name}\r\n";
				int endIndex = targetCode.IndexOf(endingKey) + endingKey.Length;
				int stringLength = endIndex - startIndex;

				if (startIndex >= targetCode.Length || startIndex < 0 || stringLength < 0 || stringLength >= targetCode.Length) return;

				targetCode = targetCode.Remove(startIndex, stringLength);
				File.WriteAllText(scriptPath, targetCode);

				if (recompileScript)
                {
					AssetDatabase.SaveAssets();
					AssetDatabase.ImportAsset(Path.Combine(GlobalPaths.RelativeScriptsFolderPath, "CustomContextMenuItems.cs"));
				}

				ScriptBuilder.Log("Template removed!");
			}
			else
			{
				ScriptBuilder.LogError("Important Source Files not found! Consider reimporting SpeedScript.");
			}
		}

		private void DeleteTemplateFile(string path)
		{
			if (File.Exists(path)) File.Delete(path);
		}

		#endregion

			#region OTHER

		public static void RefreshCustomTemplates()
        {
			EditorApplication.update -= RefreshCustomTemplates;

			string codePath = Path.Combine(GlobalPaths.ScriptsFolderPath, "CustomContextMenuItems.cs");

			if (!File.Exists(codePath))
			{
				ScriptBuilder.LogError("Important Source Files missing! Consider reinstalling SpeedScript!");
				return;
			}

			LoadUserTemplates();

			string[] menuItems = LoadActiveContextMenus();
			List<string> newMenuItems = new List<string>();
			bool recompile = false;


			//GO THROUGH NAMES AND ADD MISSING CONTEXT MENUS
			if (_userTemplateNames != null)
            {
				for (int i = 0; i < _userTemplateNames.Length; i++)
				{
					string name = _userTemplateNames[i];

					if (menuItems == null || !ArrayContains(menuItems, name))
					{
						newMenuItems.Add(name);
						string displayName = SetDisplayName(name);
						bool editorTemplate = IsEditorTemplate(_userTemplatePaths[i]);

						AddTemplateContextMenu(_userTemplateNames[i], displayName, editorTemplate, false);
						recompile = true;
					}
				}
			}


			//GO THROUGH CONTEXT MENU AND REMOVE ITEMS FOR MISSING FILES
			string[] currentMenuNames = menuItems;

			if (menuItems != null)
            {
				for (int i = 0; i < currentMenuNames.Length; i++)
				{
					if (string.IsNullOrEmpty(currentMenuNames[i]))
                    {
						menuItems[i] = string.Empty;
						continue;
                    }

					bool fileExists = false;

					if (_userTemplateNames != null && ArrayContains(_userTemplateNames, currentMenuNames[i]))
					{
						fileExists = true;
					}

					if (!fileExists)
					{
						RemoveTemplateContextMenu(currentMenuNames[i], false);
						menuItems[i] = string.Empty;
						recompile = true;
					}
				}
			}
            
			List<string> allMenuItems = new List<string>();
			allMenuItems.AddRange(menuItems);
			allMenuItems.AddRange(newMenuItems);
			SaveActiveContextMenus(allMenuItems);

			//RECOMPILE
			if (recompile)
            {
				AssetDatabase.SaveAssets();
				AssetDatabase.ImportAsset(Path.Combine(GlobalPaths.RelativeScriptsFolderPath, "CustomContextMenuItems.cs"));
				ScriptBuilder.Log("Reimported Custom Templates succesfully");
			}
		}

		private static string SetDisplayName(string name)
		{
			string displayName = name;
			for (int i = 0; i < name.Length; i++)
			{
				if (i > 0 && char.IsUpper(displayName[i]))
				{
					displayName = displayName.Insert(i, " ");
					i++;
				}
			}

			return displayName;
		}

		private static bool IsEditorTemplate(string templatePath)
        {
			string fileName = GlobalPaths.GetFolderNameFromPath(templatePath);
			fileName = fileName.Replace("Template_", string.Empty);
			fileName = fileName.Replace(".cs.txt", string.Empty);

			if (fileName.StartsWith("Editor_"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void TryOpenSourceFolder()
        {
            string path = GlobalPaths.UserTemplatesFolderPath;
			if (Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			System.Diagnostics.Process.Start(path);
        }

		private void OpenTemplateFile(string path)
		{
			System.Diagnostics.Process.Start(path);
		}

			#endregion

        #endregion

        #region DATA MANAGEMENT

		private void RemoveWhitespacesFromNamespaces()
        {
			_config.DefaultNamespace = _config.DefaultNamespace.Replace(" ", string.Empty);
        }

		private void LoadEditorPrefs()
        {
			if (EditorPrefs.HasKey("NamespaceFoldout"))
            {
				_namespaceFoldout = EditorPrefs.GetBool("NamespaceFoldout");
            }

			if (EditorPrefs.HasKey("TemplateFoldout"))
            {
				_templateFoldout = EditorPrefs.GetBool("TemplateFoldout");
            }
        }

		private void SaveEditorPrefs()
        {
			EditorPrefs.SetBool("NamespaceFoldout", _namespaceFoldout);
			EditorPrefs.SetBool("TemplateFoldout", _templateFoldout);
		}

		private static void LoadUserTemplates()
        {
			string path = GlobalPaths.UserTemplatesFolderPath;

			if (!Directory.Exists(path))
            {
				Directory.CreateDirectory(path);
				_userTemplateNames = null;
				_userTemplatePaths = null;
				_userTemplatesIsEditor = null;
				return;
            }

			_userTemplatePaths = Directory.GetFiles(path, "Template_*.cs.txt");

			if (_userTemplatePaths != null && _userTemplatePaths.Length > 0)
            {
				_userTemplateNames = new string[_userTemplatePaths.Length];
				_userTemplatesIsEditor = new bool[_userTemplatePaths.Length];

				for (int i = 0; i < _userTemplatePaths.Length; i++)
				{
					string fileName = GlobalPaths.GetFolderNameFromPath(_userTemplatePaths[i]);
					fileName = fileName.Replace("Template_", string.Empty);
					fileName = fileName.Replace(".cs.txt", string.Empty);

					if (fileName.StartsWith("Editor_"))
                    {
						fileName = fileName.Replace("Editor_", string.Empty);
						_userTemplatesIsEditor[i] = true;
                    }
                    else
                    {
						_userTemplatesIsEditor[i] = false;
                    }

					_userTemplateNames[i] = fileName;
				}
			}
            else
            {
				_userTemplateNames = null;
            }
        }

		private static string[] LoadActiveContextMenus()
        {
			string path = GlobalPaths.ActiveMenuItemsPath;

			if (File.Exists(path))
			{
				return File.ReadAllLines(path);
			}

			return null;
		}

		private static void SaveActiveContextMenus(List<string> content)
        {
			string path = GlobalPaths.ActiveMenuItemsPath;

			string text = "";

            for (int i = 0; i < content.Count; i++)
            {
				if (!string.IsNullOrEmpty(content[i]))
                {
					text += $"{content[i]}\r\n";
                }
            }

			File.WriteAllText(path, text);
		}

		private static bool ArrayContains(string[] array, string searchText)
        {
            for (int i = 0; i < array.Length; i++)
            {
				if (array[i] == searchText)
                {
					return true;
                }
            }

			return false;
        }

        #endregion
	}
}