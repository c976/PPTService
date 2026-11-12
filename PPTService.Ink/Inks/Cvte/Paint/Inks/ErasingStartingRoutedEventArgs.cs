using System.Windows;

namespace Cvte.Paint.Inks
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
