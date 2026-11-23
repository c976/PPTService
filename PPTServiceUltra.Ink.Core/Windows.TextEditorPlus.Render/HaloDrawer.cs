using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Glyph;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class HaloDrawer : DrawerBase
	{
		internal override void DrawEffect(DrawType drawType = DrawType.TextChar)
		{
			HaloEffectArgs haloEffectArgs = EffectArgs as HaloEffectArgs;
			if (haloEffectArgs == null)
			{
				throw new ArgumentNullException();
			}
			DrawingContext drawingContext = DrawingVisual.RenderOpen();
			SolidColorBrush brush = new SolidColorBrush(haloEffectArgs.HaloColor);
			switch (drawType)
			{
			case DrawType.TextChar:
				if (TextCharVisuals.Any((TextCharVisual visual) => visual.GlyphData == null))
				{
					GeometryGroup geometryGroup = GetGeometryGroup();
					drawingContext.DrawGeometry(brush, null, geometryGroup);
				}
				else
				{
					drawingContext.DrawingTextCharVisuals(TextCharVisuals, brush);
				}
				break;
			case DrawType.Underline:
				DrawUnderline(haloEffectArgs, drawingContext);
				break;
			}
			drawingContext.Close();
			DrawingVisual.Effect = new DropShadowEffect
			{
				BlurRadius = haloEffectArgs.HaloWeight,
				Opacity = haloEffectArgs.HaloOpacity,
				Color = haloEffectArgs.HaloColor,
				Direction = 315.0,
				ShadowDepth = 0.0,
				RenderingBias = RenderingBias.Performance
			};
			DrawingVisual.Opacity = haloEffectArgs.Opacity;
		}

		private void DrawUnderline(HaloEffectArgs haloArgs, DrawingContext drawingContext)
		{
			ValueTuple<Point, Point> underlinePoint = GetUnderlinePoint();
			Point item = underlinePoint.Item1;
			Point item2 = underlinePoint.Item2;
			SolidColorBrush brush = new SolidColorBrush(haloArgs.HaloColor)
			{
				Opacity = haloArgs.Opacity
			};
			drawingContext.DrawLine(new Pen(brush, UnderlineRenderData.UnderlineBounds.Height)
			{
				LineJoin = PenLineJoin.Round
			}, item, item2);
		}
	}
}
