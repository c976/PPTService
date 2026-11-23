using System;
using System.Collections.Generic;

namespace PPTServiceUltra.Paint.Framework
{
	public class SlidesEventArgs : EventArgs
	{
		public IEnumerable<Slide> Slides { get; private set; }

		public SlidesEventArgs(IEnumerable<Slide> slideses)
		{
			Slides = slideses;
		}
	}
}
