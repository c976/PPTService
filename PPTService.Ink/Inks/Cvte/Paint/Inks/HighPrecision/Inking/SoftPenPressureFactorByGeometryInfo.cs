using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Inks.HighPrecision.Inking
{
	public class SoftPenPressureFactorByGeometryInfo
	{
		public Geometry CalculatePressureFactorGeometry { get; }

		public double LineHeight { get; }

		public double MinX { get; }

		public double MaxX { get; }

		public SoftPenPressureFactorByGeometryInfo(Geometry calculatePressureFactorGeometry, double minX, double maxX, double lineHeight = 400.0)
		{
			CalculatePressureFactorGeometry = calculatePressureFactorGeometry;
			LineHeight = lineHeight;
			MinX = minX;
			MaxX = maxX;
		}

		public float ConvertPressureFactor(float originPressureFactor)
		{
			List<Point> pointsOnPath = PointOnGeometryHelper.GetPointsOnPath((MaxX - MinX) * (double)originPressureFactor + MinX, CalculatePressureFactorGeometry);
			double num = 0.0;
			if (pointsOnPath.Count > 0)
			{
				num = LineHeight - pointsOnPath[0].Y;
			}
			return (float)(num / LineHeight);
		}
	}
}
