using System.Windows;

namespace Cvte.Paint.Framework
{
	public class SlideChangeEventArgs : SlideEventArgs
	{
		public DependencyProperty Property { get; private set; }

		public SlideChangeEventArgs(Slide slide, DependencyProperty property)
			: base(slide)
		{
			Property = property;
		}
	}
}
