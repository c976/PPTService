namespace Cvte.Paint.Framework
{
	public class CurrentSlideChangeEventArgs : ValueChangeEventArgs<Slide>
	{
		public CurrentSlideChangeEventArgs(Slide oldValue, Slide newValue)
			: base(oldValue, newValue)
		{
		}
	}
}
