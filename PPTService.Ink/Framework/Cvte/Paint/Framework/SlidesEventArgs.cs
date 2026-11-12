using System;
using System.Collections.Generic;

namespace Cvte.Paint.Framework
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
