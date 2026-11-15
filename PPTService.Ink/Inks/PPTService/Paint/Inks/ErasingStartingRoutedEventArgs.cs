using System.Windows;

namespace PPTService.Paint.Inks
{
	public class ErasingStartingRoutedEventArgs : ErasingRoutedEventArgs
	{
		public ErasingStartingRoutedEventArgs(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		public ErasingStartingRoutedEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
		}
	}
}
