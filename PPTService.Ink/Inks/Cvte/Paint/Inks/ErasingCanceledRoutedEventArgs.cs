using System.Windows;

namespace Cvte.Paint.Inks
{
	public class ErasingCanceledRoutedEventArgs : ErasingRoutedEventArgs
	{
		public ErasingCanceledRoutedEventArgs(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		public ErasingCanceledRoutedEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
		}
	}
}
