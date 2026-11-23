using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class WaveLineTextEditorDecoration : TextEditorDecoration
	{
		public WaveLineTextEditorDecoration()
			: base(TextDecorationLocation.Underline)
		{
		}

		public override Drawing OnBuildDecoration(Rect bounds, Brush foreground)
		{
			WaveLine waveLine = new WaveLine
			{
				WaveHeight = bounds.Height,
				WaveLength = bounds.Height * 2.0,
				BorderThickness = bounds.Height / 5.0
			};
			double num = bounds.Height / 2.0;
			Point startPoint = new Point(bounds.Left, bounds.Y + num);
			Point endPoint = new Point(bounds.Right, bounds.Y + num);
			StreamGeometry geometry = waveLine.DrawWaveLine(startPoint, endPoint);
			return new GeometryDrawing(null, new Pen(foreground, waveLine.BorderThickness), geometry);
		}
	}
}
