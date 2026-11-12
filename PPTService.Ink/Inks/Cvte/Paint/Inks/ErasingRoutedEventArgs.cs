using System.Windows;

namespace Cvte.Paint.Inks
{
	public class ErasingRoutedEventArgs : RoutedEventArgs
	{
		public ErasingRoutedEventArgs(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		public ErasingRoutedEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
		}
	}
}
