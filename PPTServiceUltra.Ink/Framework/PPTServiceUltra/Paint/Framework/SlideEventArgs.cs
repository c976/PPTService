using System;

namespace PPTServiceUltra.Paint.Framework
{
	public class SlideEventArgs : EventArgs
	{
		public Slide Slide { get; private set; }

		public SlideEventArgs(Slide slide)
		{
			Slide = slide;
		}
	}
}
