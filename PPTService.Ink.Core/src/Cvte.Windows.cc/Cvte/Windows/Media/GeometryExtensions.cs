using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.Media
{
	public static class GeometryExtensions
	{
		public static IEnumerable<Point> GetPoints(this Geometry geometry)
		{
			PathSegment[] source = geometry.GetFlattenedPathGeometry().Figures.SelectMany((PathFigure x) => x.Segments).ToArray();
			return source.OfType<PolyLineSegment>().SelectMany((PolyLineSegment x) => x.Points).Concat(from x in source.OfType<LineSegment>()
				select x.Point);
		}

		public static StreamGeometry ConvertToGeometry(this IEnumerable<Point> points)
		{
			StreamGeometry streamGeometry = new StreamGeometry();
			using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
			{
				points = (points as ICollection<Point>) ?? points.ToArray();
				if (points.Any())
				{
					streamGeometryContext.BeginFigure(points.First(), true, true);
					foreach (Point item in points.Skip(1))
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
