using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public class PathResolveInfos
	{
		public BezierInfos BezierInfos { get; } = new BezierInfos();

		public ArcInfos ArcInfos { get; } = new ArcInfos();

		public List<Point> Points { get; set; }
	}
}
