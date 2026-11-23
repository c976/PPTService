using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Glyph;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class ShadowDrawer : DrawerBase
	{
		internal override void DrawEffect(DrawType drawType = DrawType.TextChar)
		{
			ShadowEffectArgs shadowEffectArgs = EffectArgs as ShadowEffectArgs;
			if (shadowEffectArgs == null)
			{
				throw new ArgumentException("EffectArgs is not ShadowEffectArgs and actual is " + (EffectArgs?.GetType().FullName ?? "<null>"));
			}
			if (DrawingVisual == null)
			{
				TextTracer.Warning("[ShadowDrawer.DrawEffect] DrawingVisual is null", "ShadowDrawer", "DrawEffect");
				return;
			}
			DrawingContext drawingContext = DrawingVisual.RenderOpen();
			shadowEffectArgs.ShadowBrush = (shadowEffectArgs.ShadowBrush ?? Brushes.Black).Clone();
			shadowEffectArgs.ShadowBrush.Opacity = shadowEffectArgs.Opacity;
			switch (drawType)
			{
			case DrawType.TextChar:
				if (TextCharVisuals.Any((TextCharVisual visual) => visual.GlyphData == null))
				{
					GeometryGroup geometryGroup = GetGeometryGroup();
					drawingContext.DrawGeometry(shadowEffectArgs.ShadowBrush, null, geometryGroup);
				}
				else
				{
					drawingContext.DrawingTextCharVisuals(TextCharVisuals, shadowEffectArgs.ShadowBrush);
				}
				break;
			case DrawType.Underline:
				DrawUnderline(shadowEffectArgs, drawingContext);
				break;
			}
			drawingContext.Close();
			DrawingVisual.Effect = new DropShadowEffect
			{
				RenderingBias = RenderingBias.Performance,
				BlurRadius = shadowEffectArgs.ShadowBlur,
				ShadowDepth = shadowEffectArgs.ShadowDistance,
				Opacity = shadowEffectArgs.ShadowOpacity * shadowEffectArgs.Opacity,
				Direction = shadowEffectArgs.ShadowDirection,
				Color = ((shadowEffectArgs.ShadowBrush as SolidColorBrush)?.Color ?? Colors.Gainsboro)
			};
			DrawingVisual.Opacity = shadowEffectArgs.Opacity;
		}

		private void DrawUnderline(ShadowEffectArgs shadowArgs, DrawingContext drawingContext)
		{
			ValueTuple<Point, Point> underlinePoint = GetUnderlinePoint();
			Point item = underlinePoint.Item1;
			Point item2 = underlinePoint.Item2;
			drawingContext.DrawLine(new Pen(shadowArgs.ShadowBrush, UnderlineRenderData.UnderlineBounds.Height)
			{
				DashStyle = UnderlineRenderData.DashStyle
			}, item, item2);
		}
	}
}
