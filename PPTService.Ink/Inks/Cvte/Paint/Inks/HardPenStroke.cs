using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Inks.Storage;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class HardPenStroke : StrokeBase<HardPenInkSaveInfo>
	{
		[UsedImplicitly]
		protected HardPenStroke()
		{
		}

		public HardPenStroke(StylusPointCollection stylusPoints, InkBoardSettings state)
			: this(stylusPoints, state.InkColor, state.InkThickness * state.InkScale)
		{
		}

		public HardPenStroke(StylusPointCollection stylusPoints, Color strokeColor, double strokeWidth)
			: base(stylusPoints)
		{
			InitStroke(stylusPoints, strokeColor, strokeWidth);
		}

		private void InitStroke(StylusPointCollection stylusPoints, Color strokeColor, double strokeWidth)
		{
			DrawingAttributes.Width = strokeWidth;
			DrawingAttributes.Height = strokeWidth;
			DrawingAttributes.Color = strokeColor;
			StylusPoints = stylusPoints;
		}

		public override HardPenInkSaveInfo GetSaveInfo()
		{
			return new HardPenInkSaveInfo
			{
				Thickness = DrawingAttributes.Width,
				Foreground = DrawingAttributes.Color,
				Points = StylusPoints
			};
		}

		public override void SetSaveInfo(HardPenInkSaveInfo saveInfo)
		{
			DrawingAttributes.Width = saveInfo.Thickness;
			DrawingAttributes.Height = saveInfo.Thickness;
			DrawingAttributes.Color = saveInfo.Foreground;
			StylusPoints = saveInfo.Points;
		}

		private void DrawMark(DrawingContext drawingContext, StylusPoint point, Color color, double size)
		{
			Vector vector = new Vector(size / 2.0, size / 2.0);
			Point point2 = point.ToPoint();
			SolidColorBrush brush = new SolidColorBrush(color);
			drawingContext.DrawRectangle(brush, null, new Rect(point2 - vector, point2 + vector));
		}
	}
}
