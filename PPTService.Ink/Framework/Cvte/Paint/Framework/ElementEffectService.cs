using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Cvte.Paint.Framework
{
	public static class ElementEffectService
	{
		public static DrawingVisual CreateReflectionVisual(Visual element, Rect bounds, ReflectionInfo reflectionInfo, Rect correctRect)
		{
			return CreateReflection(element, new Point(0.0, bounds.Bottom), reflectionInfo, correctRect);
		}

		private static DrawingVisual CreateReflection(Visual element, Point scalePoint, ReflectionInfo reflectionInfo, Rect rect)
		{
			VisualBrush visualBrush = new VisualBrush();
			visualBrush.AlignmentX = AlignmentX.Left;
			visualBrush.AlignmentY = AlignmentY.Top;
			visualBrush.Visual = element;
			visualBrush.Stretch = Stretch.None;
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				drawingContext.DrawRectangle(visualBrush, null, rect);
			}
			drawingVisual.Transform = new ScaleTransform(1.0, -1.0, scalePoint.X, scalePoint.Y + reflectionInfo.OffsetY / 2.0);
			drawingVisual.OpacityMask = reflectionInfo.OpacityMask;
			drawingVisual.Opacity = reflectionInfo.Opacity;
			return drawingVisual;
		}

		public static DrawingVisual CreateShadow(Geometry geometry, ShadowInfo shadowInfo, double rotation, Point center, double thickness, bool isTransparent, double opacity)
		{
			DrawingVisual drawingVisual = new DrawingVisual();
			Geometry shadowGeometry = GetShadowGeometry(geometry, shadowInfo, rotation, center);
			using (DrawingContext drawingContext = drawingVisual.RenderOpen())
			{
				Pen pen = new Pen(shadowInfo.ShadowBrush, thickness);
				if (shadowInfo.IsDropShadow)
				{
					if (Math.Abs(shadowInfo.Angle - 360.0) < 0.1)
					{
						pen = new Pen(shadowInfo.ShadowBrush, shadowInfo.ArroundOffset + thickness);
					}
					if (isTransparent && thickness < 0.1)
					{
						pen = new Pen(shadowInfo.ShadowBrush, thickness);
					}
				}
				drawingContext.DrawGeometry(isTransparent ? Brushes.Transparent : shadowInfo.ShadowBrush, pen, shadowGeometry);
			}
			drawingVisual.Effect = new BlurEffect
			{
				RenderingBias = RenderingBias.Performance,
				Radius = shadowInfo.BlurRadius
			};
			if (shadowInfo.IsDropShadow && Math.Abs(shadowInfo.Angle - 360.0) > 0.1)
			{
				drawingVisual.Transform = new TranslateTransform(shadowInfo.ArroundOffsetX, shadowInfo.ArroundOffsetY);
			}
			drawingVisual.Opacity = shadowInfo.Opacity * opacity;
			return drawingVisual;
		}

		private static Geometry GetShadowGeometry(Geometry geometry, ShadowInfo shadowInfo, double rotation, Point center)
		{
			Geometry geometry2 = geometry.Clone();
			TransformGroup transformGroup = new TransformGroup();
			if (geometry2.Transform != null)
			{
				transformGroup.Children.Add(geometry2.Transform);
			}
			transformGroup.Children.Add(new RotateTransform(rotation, center.X, center.Y));
			geometry2.Transform = transformGroup;
			Rect bounds = geometry2.Bounds;
			if (shadowInfo.ShadowTransform != null)
			{
				transformGroup.Children.Add(shadowInfo.ShadowTransform);
				geometry2.Transform = transformGroup;
				((TransformGroup)geometry2.Transform).Children.Add(new TranslateTransform(shadowInfo.XScale * bounds.Bottom, shadowInfo.IsUp ? (bounds.Bottom - geometry2.Bounds.Bottom + shadowInfo.OffsetY) : (bounds.Bottom - geometry2.Bounds.Top + shadowInfo.OffsetY)));
			}
			return geometry2;
		}
	}
}
