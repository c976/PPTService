using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Paint.Framework
{
	public class ElementClipGeometryChangedEventArgs : RoutedEventArgs
	{
		public Element Element { get; private set; }

		public Geometry NewClipGeometry { get; }

		public Geometry OldClipGeometry { get; }

		public ElementClipGeometryChangedEventArgs(RoutedEvent routedEvent, Element element, Geometry oldClipGeometry, Geometry newClipGeometry)
			: base(routedEvent, element)
		{
			Element = element;
			OldClipGeometry = oldClipGeometry;
			NewClipGeometry = newClipGeometry;
		}
	}
}
