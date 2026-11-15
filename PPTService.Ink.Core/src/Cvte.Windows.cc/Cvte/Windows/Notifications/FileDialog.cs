namespace PPTService.Ink.Core.Windows.Notifications
{
	public class FileDialog
	{
		public FileDialogType Type;

		public object Tag;

		public bool Multiselect;

		public bool AddExtension = true;

		public string DefaultExt = string.Empty;

		public string SafeFileName = string.Empty;

		public string[] SafeFileNames = new string[1] { string.Empty };

		public string FileName = string.Empty;

		public string[] FileNames = new string[1] { string.Empty };

		public string Filter = string.Empty;

		public int FilterIndex = 1;

		public string InitialDirectory = string.Empty;

		public string Title = string.Empty;
	}
}
