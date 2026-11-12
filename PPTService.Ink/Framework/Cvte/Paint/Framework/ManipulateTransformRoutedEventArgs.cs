using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public abstract class ManipulateTransformRoutedEventArgs : ApplyingTransformRoutedEventArgs
	{
		public IEnumerable<string> ElementIds { get; }

		public Matrix TransformMatrix { get; set; }

		public bool IsAbsoluteMatrix { get; set; } = true;

		[Obsolete("请改用 IsAbsoluteMatrix 属性。", true)]
		public bool isAbsoluteMatrix
		{
			get
			{
				return IsAbsoluteMatrix;
			}
			set
			{
				IsAbsoluteMatrix = value;
			}
		}

		public Vector BoardDeltaOffset { get; set; }

		public double BoardDeltaScale { get; set; }

		protected ManipulateTransformRoutedEventArgs(RoutedEvent routedEvent, Slide slide, IEnumerable<Element> elements)
			: base(routedEvent, slide, elements, TransformBehavior.Manipulate, TransformBehavior.Manipulate)
		{
			ElementIds = elements.Select((Element x) => x.Id);
		}
	}
}
