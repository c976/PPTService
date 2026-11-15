using System.Collections.Generic;

namespace PPTService.Paint.Framework
{
	public class ArcInfos
	{
		public List<int> StartIndexes { get; set; } = new List<int>();

		public List<ArcInfo> Infos { get; set; } = new List<ArcInfo>();
	}
}
