using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Framework
{
	public class ManipulateEndRoutedEventArgs : ManipulateTransformRoutedEventArgs
	{
		public ManipulateEndRoutedEventArgs(Slide slide, IEnumerable<Element> elements)
			: base(Transformer.TransformCompletedEvent, slide, elements)
		{
		}
	}
}
