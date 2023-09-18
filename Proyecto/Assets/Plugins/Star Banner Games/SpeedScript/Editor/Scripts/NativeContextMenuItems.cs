using UnityEditor;

namespace SBG.SpeedScript
{
	public class NativeContextMenuItems
	{
        #region TEMPLATES

        [MenuItem(GlobalPaths.TEMPLATE_MENUPATH + "Mono Behaviour", false, ScriptBuilder.TEMPLATE_NORMAL_MENU_PRIORITY)]
        public static void CreateDefaultClass(MenuCommand cmd)
        {
            ScriptBuilder.CreateScriptFromTemplate("Template_MonoBehaviour", "NewMonoBehaviour");
        }

        [MenuItem(GlobalPaths.TEMPLATE_MENUPATH + "Empty Class", false, ScriptBuilder.TEMPLATE_NORMAL_MENU_PRIORITY)]
        public static void CreateEmptyClass(MenuCommand cmd)
        {
            ScriptBuilder.CreateScriptFromTemplate("Template_EmptyClass", "NewEmptyClass");
        }

        [MenuItem(GlobalPaths.TEMPLATE_MENUPATH + "Struct", false, ScriptBuilder.TEMPLATE_NORMAL_MENU_PRIORITY)]
        public static void CreateStruct(MenuCommand cmd)
        {
            ScriptBuilder.CreateScriptFromTemplate("Template_Struct", "NewStruct");
        }

        [MenuItem(GlobalPaths.TEMPLATE_MENUPATH + "Enum", false, ScriptBuilder.TEMPLATE_NORMAL_MENU_PRIORITY)]
        public static void CreateEnum(MenuCommand cmd)
        {
            ScriptBuilder.CreateScriptFromTemplate("Template_Enum", "NewEnum");
        }

        [MenuItem(GlobalPaths.TEMPLATE_MENUPATH + "Interface", false, ScriptBuilder.TEMPLATE_NORMAL_MENU_PRIORITY)]
        public static void CreateInterface(MenuCommand cmd)
        {
            ScriptBuilder.CreateScriptFromTemplate("Template_Interface", "NewInterface");
        }

        [MenuItem(GlobalPaths.TEMPLATE_MENUPATH + "Scriptable Object", false, ScriptBuilder.TEMPLATE_NORMAL_MENU_PRIORITY)]
        public static void CreateScriptableObject(MenuCommand cmd)
        {
            ScriptBuilder.CreateScriptFromTemplate("Template_ScriptableObject", "NewScriptableObject");
        }

        #endregion

        #region EDITOR TEMPLATES

        //[MenuItem(GlobalPaths.TEMPLATE_MENUPATH, false, ScriptBuilderEditorWindow.TEMPLATE_EDITOR_MENU_PRIORITY)]

        [MenuItem(GlobalPaths.TEMPLATE_EDITOR_MENUPATH + "EditorWindow", false, ScriptBuilder.TEMPLATE_EDITOR_MENU_PRIORITY)]
        public static void CreateEditorWindow(MenuCommand cmd)
        {
            ScriptBuilder.CreateScriptFromTemplate("Template_EditorWindow", "NewEditorWindow");
        }

        [MenuItem(GlobalPaths.TEMPLATE_EDITOR_MENUPATH + "Custom Inspector", false, ScriptBuilder.TEMPLATE_EDITOR_MENU_PRIORITY)]
        public static void CreateCustomInspector(MenuCommand cmd)
        {
            ScriptBuilder.CreateScriptFromTemplate("Template_CustomInspector", "NewCustomInspector");
        }

        #endregion
    }
}