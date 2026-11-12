using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public class SlidesChangeEventArgs : SlidesEventArgs
	{
		public DependencyProperty Property { get; private set; }

		public SlidesChangeEventArgs(IEnumerable<Slide> slides, DependencyProperty property)
			: base(slides)
		{
			Property = property;
		}
	}
}
