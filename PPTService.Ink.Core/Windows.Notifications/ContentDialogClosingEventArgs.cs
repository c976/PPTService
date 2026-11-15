using System;

namespace PPTService.Ink.Core.Windows.Notifications
{
	public sealed class ContentDialogClosingEventArgs
	{
		private ContentDialogClosingDeferral _deferral;

		public bool Cancel { get; set; }

		public ContentDialogResult Result { get; }

		internal ContentDialogClosingEventArgs(ContentDialogResult result)
		{
			Result = result;
			_deferral = new ContentDialogClosingDeferral(() =>
			{
				throw new NotImplementedException();
			});
		}

		public ContentDialogClosingDeferral GetDeferral()
		{
			return _deferral;
		}
	}
}
