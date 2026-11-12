using System.Windows;

namespace Cvte.Paint.Framework
{
	public class ElementRoutedEventArgs : RoutedEventArgs
	{
		public new Element Source
		{
			get
			{
				return (Element)base.Source;
			}
			set
			{
				base.Source = value;
			}
		}

		public new Element OriginalSource => (Element)base.OriginalSource;

		public ElementRoutedEventArgs()
		{
		}

		public ElementRoutedEventArgs(RoutedEvent routedEvent)
			: base(routedEvent)
		{
		}

		public ElementRoutedEventArgs(RoutedEvent routedEvent, object source)
			: base(routedEvent, source)
		{
		}
	}
}
