using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Glyph
{
	public static class GlyphRunCreator
	{
		private static readonly Point[] DefaultGlyphOffsetArray = new Point[1];

		private static readonly XmlLanguage DefaultXmlLanguage = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

		public static bool TryGetGlyphInfo([NotNull] Typeface typeface, char unicodeChar, out GlyphInfo info)
		{
			if (typeface == null)
			{
				throw new ArgumentNullException("typeface");
			}
			ushort value = 0;
			info = null;
			GlyphTypeface glyphTypeface;
			if (typeface.TryGetGlyphTypeface(out glyphTypeface) && glyphTypeface.CharacterToGlyphMap.TryGetValue(unicodeChar, out value))
			{
				info = new GlyphInfo(typeface, glyphTypeface, glyphTypeface, unicodeChar, value);
				return true;
			}
			string familyName;
			GlyphTypeface glyphTypeface2;
			if (FallBackFontFamily.Instance.TryGetFallBackFontFamily(unicodeChar, out familyName) && new Typeface(new FontFamily(familyName), typeface.Style, typeface.Weight, typeface.Stretch).TryGetGlyphTypeface(out glyphTypeface2) && glyphTypeface2.CharacterToGlyphMap.TryGetValue(unicodeChar, out value))
			{
				GlyphTypeface originGlyphTypeface = glyphTypeface ?? glyphTypeface2;
				info = new GlyphInfo(typeface, glyphTypeface2, originGlyphTypeface, unicodeChar, value);
				return true;
			}
			return false;
		}

		public static GlyphRun BuildSingleGlyphRun([NotNull] GlyphInfo info, double fontSize)
		{
			return BuildSingleGlyphRun(info, fontSize, default(Point));
		}

		public static GlyphRun BuildSingleGlyphRun([NotNull] GlyphInfo info, double fontSize, Point baselineOrigin)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			double i = info.RenderGlyphTypeface.AdvanceWidths[info.GlyphIndex] * fontSize;
			i = GlyphExtension.RefineValue(i);
			return new GlyphRun(info.RenderGlyphTypeface, 0, false, fontSize, new ushort[1] { info.GlyphIndex }, baselineOrigin, new double[1] { i }, DefaultGlyphOffsetArray, new char[1] { info.UnicodeChar }, null, null, null, DefaultXmlLanguage);
		}
	}
}
