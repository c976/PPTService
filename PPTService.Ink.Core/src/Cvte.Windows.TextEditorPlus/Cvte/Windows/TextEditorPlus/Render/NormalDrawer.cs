using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Glyph;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class NormalDrawer : DrawerBase
	{
		internal override void DrawEffect(DrawType drawType = DrawType.TextChar)
		{
			DrawingContext drawingContext = DrawingVisual.RenderOpen();
			switch (drawType)
			{
			case DrawType.TextChar:
				if (TextCharVisuals.Any((TextCharVisual visual) => visual.GlyphData == null))
				{
					GeometryGroup geometryGroup = GetGeometryGroup();
					drawingContext.DrawGeometry(TextCharVisuals[0].Foreground, null, geometryGroup);
				}
				else
				{
					drawingContext.DrawingTextCharVisuals(TextCharVisuals, TextCharVisuals[0].Foreground);
				}
				break;
			case DrawType.Underline:
				DrawUnderline(drawingContext);
				break;
			}
			drawingContext.Close();
			DrawingVisual.Opacity = EffectArgs.Opacity;
		}

		private void DrawUnderline(DrawingContext drawingContext)
		{
			ValueTuple<Point, Point> underlinePoint = GetUnderlinePoint();
			Point item = underlinePoint.Item1;
			Point item2 = underlinePoint.Item2;
			drawingContext.DrawLine(new Pen(TextCharVisuals.First().Foreground, UnderlineRenderData.UnderlineBounds.Height)
			{
				DashStyle = UnderlineRenderData.DashStyle
			}, item, item2);
		}
	}
}
