using System;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils.Glyph
{
	internal static class GlyphExtension
	{
		private const double DefaultRealToIdeal = 300.0;

		private const double DefaultIdealToReal = 1.0 / 300.0;

		public static Rect GetBounds(this GlyphRun run)
		{
			Rect result = run.ComputeInkBoundingBox();
			if (result.Width >= 2.0 && result.Height >= 2.0)
			{
				result.Inflate(-1.0, -1.0);
			}
			return result;
		}

		public static double GetBaseline(this FontFamily fontFamily, double fontRenderingEmSize)
		{
			return RefineValue(fontFamily.Baseline * fontRenderingEmSize);
		}

		public static Size GetSize(this GlyphRun run, double lineSpacing)
		{
			double fontRenderingEmSize = run.FontRenderingEmSize;
			double i = lineSpacing * fontRenderingEmSize;
			double num = 0.0;
			foreach (ushort glyphIndex in run.GlyphIndices)
			{
				num += run.GlyphTypeface.AdvanceWidths[glyphIndex];
			}
			num *= fontRenderingEmSize;
			i = RefineValue(i);
			num = RefineValue(num);
			return new Size(num, i);
		}

		public static double RefineValue(double i)
		{
			double num = IdealToRealWithNoRounding(RealToIdeal(i));
			if (i > 0.0)
			{
				num = Math.Max(num, 1.0 / 300.0);
			}
			return num;
		}

		private static int RealToIdeal(double i)
		{
			int num = (int)Math.Round(i * 300.0);
			if (i > 0.0)
			{
				num = Math.Max(num, 1);
			}
			return num;
		}

		private static double IdealToRealWithNoRounding(double i)
		{
			return i * (1.0 / 300.0);
		}
	}
}
