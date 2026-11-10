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
			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
			{
				if (points.Any())
				{
					streamGeometryContext.BeginFigure(points.First(), true, true);
					IEnumerable<Point> enumerable = points.Skip(1);
					foreach (Point item in enumerable)
					{
						streamGeometryContext.LineTo(item, true, true);
					}
				}
			}
			streamGeometry.Freeze();
			return streamGeometry;
		}
	}
}
