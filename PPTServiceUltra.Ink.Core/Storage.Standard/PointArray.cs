using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public class PointArray : ArrayType<Point>, IStorableType
	{
		public PointArray()
		{
		}

		public PointArray(IEnumerable<Point> items)
			: base(items)
		{
		}

		public static implicit operator PointArray(List<System.Windows.Point> points)
		{
			return new PointArray(((IEnumerable<System.Windows.Point>)points).Select((Func<System.Windows.Point, Point>)( (System.Windows.Point p) => p)));
		}

		public static implicit operator PointArray(PointCollection points)
		{
			return new PointArray(((IEnumerable<System.Windows.Point>)points).Select((Func<System.Windows.Point, Point>)( (System.Windows.Point p) => p)));
		}

		public static implicit operator List<System.Windows.Point>(PointArray points)
		{
			return ((IEnumerable<Point>)points.Items).Select((Func<Point, System.Windows.Point>)( (Point p) => p)).ToList();
		}

		public static implicit operator PointCollection(PointArray points)
		{
			if (points != null)
			{
				return new PointCollection(((IEnumerable<Point>)points.Items).Select((Func<Point, System.Windows.Point>)( (Point p) => p)));
			}
			return new PointCollection();
		}
	}
}
