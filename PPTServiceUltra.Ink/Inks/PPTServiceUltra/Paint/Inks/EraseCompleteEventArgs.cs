using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Paint.Framework;

namespace PPTServiceUltra.Paint.Inks
{
	[PublicAPI]
	public class EraseCompleteEventArgs : EraseEventArgs
	{
		public string CurrentSlideId { get; set; }

		public List<Element> DeleteInks { get; set; }

		public List<Element> NewInks { get; set; }
	}
}
