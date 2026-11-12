using System;
using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public class BezierInfos
	{
		public List<int> BezierIndexList { get; set; }

		public List<BezierType> BezierTypes { get; set; }

		[Obsolete]
		public List<int> ControlIndex { get; set; }

		public List<int> StartIndex { get; set; }

		public BezierInfos()
		{
			BezierIndexList = new List<int>();
			BezierTypes = new List<BezierType>();
			ControlIndex = new List<int>();
			StartIndex = new List<int>();
		}
	}
}
