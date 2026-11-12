using System.Windows;

namespace Cvte.Paint.Inks
{
	public class ErasingCompletedRoutedEventArgs : ErasingRoutedEventArgs
	{
		public ErasingCompletedRoutedEventArgs(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		public ErasingCompletedRoutedEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
		}
	}
}
