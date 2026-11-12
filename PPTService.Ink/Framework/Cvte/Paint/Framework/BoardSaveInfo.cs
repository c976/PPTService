using System.Collections.Generic;
using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	[SaveInfoContract("Board")]
	public class BoardSaveInfo : SaveInfo
	{
		[SaveInfoMember("SlideWidth", IsRequired = false, Description = "画板宽度")]
		public double SlideWidth { get; set; }

		[SaveInfoMember("SlideHeight", IsRequired = false, Description = "画板高度")]
		public double SlideHeight { get; set; }

		[SaveInfoMember("Slides", IsRequired = true, Description = "页面Id集合")]
		public IList<string> SlideIds { get; set; }
	}
}
