using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	[PublicAPI]
	public class EraseCompleteEventArgs : EraseEventArgs
	{
		public string CurrentSlideId { get; set; }

		public List<Element> DeleteInks { get; set; }

		public List<Element> NewInks { get; set; }
	}
}
