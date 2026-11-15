using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	internal abstract class DrawerBase
	{
		private DrawingVisual _drawingVisual;

		protected ArrangingType ArrangingType { get; set; }

		internal DrawingVisual DrawingVisual => _drawingVisual;

		protected List<TextCharVisual> TextCharVisuals { get; set; }

		protected double BaseLine { get; set; }

		protected UnderlineRenderData UnderlineRenderData { get; set; }

		protected ITextEffectArgs EffectArgs { get; set; }

		internal abstract void DrawEffect(DrawType drawType = DrawType.TextChar);

		protected GeometryGroup GetGeometryGroup()
		{
			foreach (TextCharVisual textCharVisual in TextCharVisuals)
			{
				if (textCharVisual.GlyphData != null)
				{
					Geometry geometry = textCharVisual.GlyphData.GlyphRun.BuildGeometry();
					if (!geometry.IsFrozen)
					{
						geometry.Transform = new MatrixTransform(textCharVisual.Matrix);
						textCharVisual.TextGeometry = geometry;
					}
				}
			}
			return GetGeometryGroupCore();
		}

		protected virtual GeometryGroup GetGeometryGroupCore()
		{
			Rect bound = TextCharVisuals.First().Bounds;
			GeometryGroup geometryGroup = new GeometryGroup();
			foreach (TextCharVisual textCharVisual in TextCharVisuals)
			{
				geometryGroup.Children.Add(textCharVisual.TextGeometry);
			}
			return geometryGroup;
		}

		protected ValueTuple<Point, Point> GetUnderlinePoint()
		{
			Rect bounds = TextCharVisuals.First().Bounds;
			double num = UnderlineRenderData.UnderlineBounds.Height / 2.0;
			Point item = new Point(0.0, UnderlineRenderData.UnderlineBounds.Y - bounds.Top + num);
			Point item2 = new Point(UnderlineRenderData.UnderlineBounds.Width, UnderlineRenderData.UnderlineBounds.Y - bounds.Top + num);
			return new ValueTuple<Point, Point>(item, item2);
		}

		internal void SetDataInfo(ITextEffectArgs textEffectArgs, List<TextCharVisual> textCharVisuals, double baseLine, UnderlineRenderData underlineRenderData, ArrangingType arrangingType = ArrangingType.Horizontal)
		{
			EffectArgs = textEffectArgs;
			TextCharVisuals = textCharVisuals;
			BaseLine = baseLine;
			UnderlineRenderData = underlineRenderData;
			_drawingVisual = new DrawingVisual();
			ArrangingType = arrangingType;
		}
	}
}
