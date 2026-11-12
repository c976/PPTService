using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public class ArcInfo
	{
		public Point StartPoint { get; set; }

		public Size RadiusSize { get; set; }

		public double Rotation { get; set; }

		public bool IsLargeArc { get; set; }

		public SweepDirection SweepDirection { get; set; }

		public Point EndPoint { get; set; }
	}
}
