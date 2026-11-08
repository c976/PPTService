using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Windows
{
	public static class GeometryExtensions
	{
		public static IEnumerable<Point> GetPoint(this Geometry geometry)
		{
			PathSegment[] source = geometry.GetFlattenedPathGeometry().Figures.SelectMany((PathFigure x) => x.Segments).ToArray();
			IEnumerable<Point> first = source.OfType<PolyLineSegment>().SelectMany((PolyLineSegment x) => x.Points);
			return first.Concat(from x in source.OfType<LineSegment>()
				select x.Point);
		}

		public static StreamGeometry ConvertToGeometry(this IEnumerable<Point> points)
		{
			StreamGeometry streamGeometry;
			while (true)
			{
				if (0 == 0)
				{
					streamGeometry = new StreamGeometry();
				}
				if (false)
				{
					continue;
				}
				StreamGeometryContext streamGeometryContext = streamGeometry.Open();
				try
				{
					while (points.Any())
					{
						streamGeometryContext.BeginFigure(points.First(), true, true);
						IEnumerable<Point> enumerable = points.Skip(1);
						if (false)
						{
							continue;
						}
						foreach (Point item in enumerable)
						{
							streamGeometryContext.LineTo(item, true, true);
						}
						break;
					}
				}
				finally
				{
					while (streamGeometryContext != null)
					{
						if (0 == 0 && 0 == 0)
						{
							((IDisposable)streamGeometryContext).Dispose();
							break;
						}
					}
				}
				streamGeometry.Freeze();
				if (4 != 0)
				{
					break;
				}
			}
			return streamGeometry;
		}
	}
}
