using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Glyph;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class FillDrawer : DrawerBase
	{
		internal override void DrawEffect(DrawType drawType = DrawType.TextChar)
		{
			FillEffectArgs fillEffectArgs = EffectArgs as FillEffectArgs;
			if (fillEffectArgs == null)
			{
				throw new ArgumentNullException();
			}
			DrawingContext drawingContext = DrawingVisual.RenderOpen();
			switch (drawType)
			{
			case DrawType.TextChar:
				if (TextCharVisuals.Any((TextCharVisual visual) => visual.GlyphData == null))
				{
					GeometryGroup geometryGroup = GetGeometryGroup();
					drawingContext.DrawGeometry(fillEffectArgs.FillBrush, null, geometryGroup);
				}
				else
				{
					drawingContext.DrawingTextCharVisuals(TextCharVisuals, fillEffectArgs.FillBrush);
				}
				break;
			case DrawType.Underline:
				DrawUnderline(fillEffectArgs, drawingContext);
				break;
			}
			drawingContext.Close();
			DrawingVisual.Opacity = fillEffectArgs.Opacity;
		}

		private void DrawUnderline(FillEffectArgs fillArgs, DrawingContext drawingContext)
		{
			ValueTuple<Point, Point> underlinePoint = GetUnderlinePoint();
			Point item = underlinePoint.Item1;
			Point item2 = underlinePoint.Item2;
			drawingContext.DrawLine(new Pen(fillArgs.FillBrush, UnderlineRenderData.UnderlineBounds.Height)
			{
				DashStyle = UnderlineRenderData.DashStyle
			}, item, item2);
		}
	}
}
