using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Framework
{
	public class ManipulateStartRoutedEventArgs : ManipulateTransformRoutedEventArgs
	{
		public ManipulateStartRoutedEventArgs(Slide slide, IEnumerable<Element> elements)
			: base(Transformer.TransformStartedEvent, slide, elements)
		{
		}
	}
}
