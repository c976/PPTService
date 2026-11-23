using System;

namespace PPTServiceUltra.Ink.Core.Windows.Notifications
{
	public sealed class ContentDialogClosingDeferral
	{
		private Action _onComplete;

		internal ContentDialogClosingDeferral(Action onComplete)
		{
			_onComplete = onComplete;
		}

		public void Complete()
		{
			_onComplete?.Invoke();
		}
	}
}
