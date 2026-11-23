using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Glyph
{
	internal static class GlyphDrawingContextExtension
	{
		public static void DrawingTextCharVisuals(this DrawingContext context, IEnumerable<TextCharVisual> textCharVisuals, Brush brush = null)
		{
			foreach (TextCharVisual item in textCharVisuals.Where((TextCharVisual visual) => visual.GlyphData != null))
			{
				TextCharGlyphData glyphData = item.GlyphData;
				Point baselineOrigin = default(Point) * item.Matrix;
				Matrix matrix = new Matrix(item.Matrix.M11, item.Matrix.M12, item.Matrix.M21, item.Matrix.M22, 0.0, 0.0);
				if (!matrix.IsIdentity)
				{
					context.PushTransform(new MatrixTransform(item.Matrix));
					try
					{
						context.DrawGlyphRun(brush ?? item.Foreground, glyphData.GlyphRun);
					}
					finally
					{
						context.Pop();
					}
				}
				else
				{
					GuidelineSet guidelineSet = new GuidelineSet();
					guidelineSet.GuidelinesX.Add(baselineOrigin.X);
					guidelineSet.GuidelinesY.Add(baselineOrigin.Y);
					context.PushGuidelineSet(guidelineSet);
					GlyphRun glyphRun = GlyphRunCreator.BuildSingleGlyphRun(glyphData.GlyphInfo, glyphData.FontSize, baselineOrigin);
					context.DrawGlyphRun(brush ?? item.Foreground, glyphRun);
					context.Pop();
				}
			}
		}
	}
}
