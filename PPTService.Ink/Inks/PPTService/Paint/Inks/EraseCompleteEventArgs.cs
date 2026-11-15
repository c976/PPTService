using System.Collections.Generic;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	[PublicAPI]
	public class EraseCompleteEventArgs : EraseEventArgs
	{
		public string CurrentSlideId { get; set; }

		public List<Element> DeleteInks { get; set; }

		public List<Element> NewInks { get; set; }
	}
}
