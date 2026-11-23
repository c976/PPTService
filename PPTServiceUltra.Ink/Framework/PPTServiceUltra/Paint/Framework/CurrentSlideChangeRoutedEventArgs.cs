using PPTServiceUltra.Ink.Core.Windows.Controls;

namespace PPTServiceUltra.Paint.Framework
{
	public class CurrentSlideChangeRoutedEventArgs : ValueChangedRoutedEventArgs<Slide>
	{
		public CurrentSlideChangeRoutedEventArgs(Slide oldValue, Slide newValue)
			: base(Board.CurrentSlideChangedEvent, oldValue, newValue)
		{
		}
	}
}
