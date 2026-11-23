using System;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class FrameDrawer : DrawerBase
	{
		internal override void DrawEffect(DrawType drawType = DrawType.TextChar)
		{
			FrameEffectArgs frameEffectArgs = EffectArgs as FrameEffectArgs;
			if (frameEffectArgs == null)
			{
				throw new ArgumentNullException();
			}
			DrawingContext drawingContext = DrawingVisual.RenderOpen();
			switch (drawType)
			{
			case DrawType.TextChar:
			{
				GeometryGroup geometryGroup = GetGeometryGroup();
				drawingContext.DrawGeometry(Brushes.Transparent, new Pen(frameEffectArgs.FrameBrush, frameEffectArgs.FrameThinkness)
				{
					LineJoin = PenLineJoin.Round
				}, geometryGroup);
				break;
			}
			case DrawType.Underline:
				DrawUnderline(frameEffectArgs, drawingContext);
				break;
			}
			drawingContext.Close();
			DrawingVisual.Opacity = frameEffectArgs.Opacity * frameEffectArgs.FrameOpacity;
		}

		private void DrawUnderline(FrameEffectArgs frameArgs, DrawingContext drawingContext)
		{
			ValueTuple<Point, Point> underlinePoint = GetUnderlinePoint();
			Point item = underlinePoint.Item1;
			Point item2 = underlinePoint.Item2;
			drawingContext.DrawLine(new Pen(frameArgs.FrameBrush, frameArgs.FrameThinkness + UnderlineRenderData.UnderlineBounds.Height)
			{
				LineJoin = PenLineJoin.Round
			}, item, item2);
		}
	}
}
