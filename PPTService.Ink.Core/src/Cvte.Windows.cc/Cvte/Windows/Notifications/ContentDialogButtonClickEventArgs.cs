namespace PPTService.Ink.Core.Windows.Notifications
{
	public sealed class ContentDialogButtonClickEventArgs
	{
		public bool Cancel { get; set; }

		public ContentDialogButtonClickDeferral GetDeferral()
		{
			return new ContentDialogButtonClickDeferral();
		}
	}
}
