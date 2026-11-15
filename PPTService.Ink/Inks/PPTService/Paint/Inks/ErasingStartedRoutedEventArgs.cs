using System.Windows;

namespace PPTService.Paint.Inks
{
	public class ErasingStartedRoutedEventArgs : ErasingRoutedEventArgs
	{
		public ErasingStartedRoutedEventArgs(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		public ErasingStartedRoutedEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
		}
	}
}
