using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public class ManipulateEndRoutedEventArgs : ManipulateTransformRoutedEventArgs
	{
		public ManipulateEndRoutedEventArgs(Slide slide, IEnumerable<Element> elements)
			: base(Transformer.TransformCompletedEvent, slide, elements)
		{
		}
	}
}
