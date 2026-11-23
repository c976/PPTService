using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	internal class ImageReloadingEventArgs : RoutedEventArgs
	{
		internal ReloadMode ReloadMode { get; set; }

		internal double Scale { get; set; } = 1.0;

		public ImageReloadingEventArgs()
		{
		}

		public ImageReloadingEventArgs(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		public ImageReloadingEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
		}
	}
}
