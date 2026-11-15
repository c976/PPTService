using System;

namespace PPTService.Paint.Framework
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
