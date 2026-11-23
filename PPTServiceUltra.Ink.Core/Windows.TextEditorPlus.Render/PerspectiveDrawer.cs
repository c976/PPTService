using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class PerspectiveDrawer : DrawerBase
	{
		internal override void DrawEffect(DrawType drawType = DrawType.TextChar)
		{
			PerspectiveEffectArgs perspectiveEffectArgs = EffectArgs as PerspectiveEffectArgs;
			if (perspectiveEffectArgs == null)
			{
				throw new ArgumentNullException();
			}
			GeometryGroup geometryGroup = GetGeometryGroup();
			Rect bounds = geometryGroup.Bounds;
			if (ArrangingType == ArrangingType.Horizontal)
			{
				Matrix value = new SkewTransform(perspectiveEffectArgs.PerspectiveAngle, 0.0, bounds.BottomRight.X, bounds.BottomRight.Y).Value;
				Matrix value2 = new ScaleTransform(1.0, perspectiveEffectArgs.PerspectiveDepth, bounds.BottomRight.X, bounds.BottomRight.Y).Value;
				geometryGroup.Transform = new MatrixTransform(value * value2);
			}
			else if (ArrangingType == ArrangingType.Vertical || ArrangingType == ArrangingType.Mongolian)
			{
				Matrix value = new SkewTransform(0.0, 0.0 - perspectiveEffectArgs.PerspectiveAngle, bounds.TopLeft.X, bounds.TopLeft.Y).Value;
				Matrix value2 = new ScaleTransform(perspectiveEffectArgs.PerspectiveDepth, 1.0, bounds.TopLeft.X, bounds.TopLeft.Y).Value;
				geometryGroup.Transform = new MatrixTransform(value * value2);
			}
			DrawingContext drawingContext = DrawingVisual.RenderOpen();
			switch (drawType)
			{
			case DrawType.TextChar:
				drawingContext.DrawGeometry(perspectiveEffectArgs.PerspectiveBrush, null, geometryGroup);
				break;
			case DrawType.Underline:
				DrawUnderline(perspectiveEffectArgs, geometryGroup, drawingContext);
				break;
			}
			drawingContext.Close();
			DrawingVisual.Effect = new BlurEffect
			{
				Radius = perspectiveEffectArgs.PerspectiveBlur,
				KernelType = KernelType.Gaussian,
				RenderingBias = RenderingBias.Performance
			};
			DrawingVisual.Opacity = perspectiveEffectArgs.PerspectiveOpacity * perspectiveEffectArgs.Opacity;
		}

		private void DrawUnderline(PerspectiveEffectArgs perspectiveArgs, GeometryGroup gg, DrawingContext drawingContext)
		{
			HandlerUnderline(perspectiveArgs, gg, drawingContext);
		}

		private void HandlerUnderline(PerspectiveEffectArgs perspectiveArgs, GeometryGroup gg, DrawingContext drawingContext)
		{
			ValueTuple<Point, Point> underlinePoint = GetUnderlinePoint();
			Point item = underlinePoint.Item1;
			Point item2 = underlinePoint.Item2;
			drawingContext.DrawLine(new Pen(perspectiveArgs.PerspectiveBrush, UnderlineRenderData.UnderlineBounds.Height)
			{
				DashStyle = UnderlineRenderData.DashStyle
			}, item, item2);
		}
	}
}
