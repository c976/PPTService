using System.Collections.Generic;

namespace PPTService.Paint.Framework
{
	public class ManipulatingRoutedEventArgs : ManipulateTransformRoutedEventArgs
	{
		public ManipulatingRoutedEventArgs(Slide slide, IEnumerable<Element> elements)
			: base(Transformer.TransformDeltaEvent, slide, elements)
		{
		}
	}
}
