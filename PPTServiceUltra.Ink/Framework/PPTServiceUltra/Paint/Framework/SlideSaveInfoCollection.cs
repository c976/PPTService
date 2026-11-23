using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Storage;

namespace PPTServiceUltra.Paint.Framework
{
	[SaveInfoContract("SlideCollection", Description = "页面存储集合")]
	public class SlideSaveInfoCollection : SaveInfo
	{
		[SaveInfoMember("Slides", Description = "页面存储集合")]
		public IList<SlideSaveInfo> Slides { get; set; }
	}
}
