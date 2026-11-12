using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public abstract class ApplyingTransformRoutedEventArgs : TransformRoutedEventArgs
	{
		public int OriginX;

		public int OriginY;

		public Rect DesiredBounds;

		public double DesiredRotation;

		protected ApplyingTransformRoutedEventArgs(RoutedEvent routedEvent, Slide slide, IEnumerable<Element> elements, TransformBehavior primaryBehavior, TransformBehavior currentBehavior)
			: base(routedEvent, slide, elements, primaryBehavior, currentBehavior)
		{
		}
	}
}
