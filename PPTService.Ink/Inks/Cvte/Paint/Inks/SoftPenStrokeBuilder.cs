using System;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace Cvte.Paint.Inks
{
	public class SoftPenStrokeBuilder
	{
		public const int RearPointCount = 8;

		private const float FirstPointPressureFactor = 0.5f;

		public Stroke BuildingStroke { get; private set; }

		public Color StrokeColor { get; set; }

		public double StrokeThickness { get; set; }

		public DrawingAttributes DrawingAttributes { get; set; }

		public bool FitToCurve { get; set; }

		public StylusPoint AddPoint(StylusPoint stylusPoint)
		{
			if (BuildingStroke == null)
			{
				BuildingStroke = CreateStroke(stylusPoint);
			}
			stylusPoint.PressureFactor /= Ratio(9);
			for (int i = 1; i <= 8 && BuildingStroke.StylusPoints.Count - i >= 1; i++)
			{
				StylusPoint value = BuildingStroke.StylusPoints[BuildingStroke.StylusPoints.Count - i];
				float val = value.PressureFactor / Ratio(9 - i) * Ratio(10 - i);
				value.PressureFactor = Math.Min(val, 1f);
				BuildingStroke.StylusPoints[BuildingStroke.StylusPoints.Count - i] = value;
			}
			BuildingStroke.StylusPoints.Add(stylusPoint);
			return stylusPoint;
		}

		private Stroke CreateStroke(StylusPoint point)
		{
			StylusPoint item = new StylusPoint(point.X, point.Y, 0.5f);
			return new Stroke(new StylusPointCollection { item }, DrawingAttributes);
		}

		private float Ratio(int level)
		{
			switch (level)
			{
			case 1:
				return 1f;
			case 2:
				return 1.25f;
			case 3:
				return 1.5f;
			case 4:
				return 1.8f;
			case 5:
				return 2.3f;
			case 6:
				return 2.9f;
			case 9:
				return 10f;
			default:
				return (float)level - 2f;
			}
		}
	}
}
