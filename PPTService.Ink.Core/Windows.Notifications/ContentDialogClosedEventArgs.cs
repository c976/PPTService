namespace PPTService.Ink.Core.Windows.Notifications
{
	public sealed class ContentDialogClosedEventArgs
	{
		public ContentDialogResult Result { get; }

		internal ContentDialogClosedEventArgs(ContentDialogResult result)
		{
			Result = result;
		}
	}
}
