using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Storage;

namespace PPTServiceUltra.Paint.Framework
{
	[SaveInfoContract("ElementCollection", Description = "元素存储集合")]
	public class ElementSaveInfoCollection : SaveInfo
	{
		[SaveInfoMember("Elements", Description = "元素存储集合")]
		public IList<ElementSaveInfo> Elements { get; set; }
	}
}
