using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public class ManipulateStartRoutedEventArgs : ManipulateTransformRoutedEventArgs
	{
		public ManipulateStartRoutedEventArgs(Slide slide, IEnumerable<Element> elements)
			: base(Transformer.TransformStartedEvent, slide, elements)
		{
		}
	}
}
