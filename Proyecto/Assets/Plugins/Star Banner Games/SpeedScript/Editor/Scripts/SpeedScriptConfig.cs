namespace SBG.SpeedScript
{
	[System.Serializable]
	public class SpeedScriptConfig
	{
		public bool DisplayedInstallDialog;
		public string DefaultNamespace;
		public string IgnoreFolder;
		public bool UseFolderNames;
		public bool LastFolderOnly;
		public bool UsePlayerSettings;


		public SpeedScriptConfig()
        {
			DisplayedInstallDialog = false;
			DefaultNamespace = "Company.Product";
			IgnoreFolder = "Scripts";
			UseFolderNames = false;
			LastFolderOnly = false;
			UsePlayerSettings = true;
        }
	}
}