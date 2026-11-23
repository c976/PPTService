using System.Windows;

namespace PPTServiceUltra.Paint.Framework
{
	public interface ILineSensitive
	{
		Point OriginPointAtSlideCoordinate { get; set; }

		double LineOriginPointScaleOfLength { get; set; }

		bool IsCurrentOriginPointOnLine { get; set; }

		bool IsLine();

		bool IsOriginPointOnLine();

		double GetOriginScale();

		Point GetOriginPoint(double scale);
	}
}
