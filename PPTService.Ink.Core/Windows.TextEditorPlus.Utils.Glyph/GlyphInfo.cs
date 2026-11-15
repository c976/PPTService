using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Glyph
{
	public class GlyphInfo
	{
		public char UnicodeChar { get; }

		public Typeface Typeface { get; }

		public GlyphTypeface RenderGlyphTypeface { get; }

		public GlyphTypeface OriginGlyphTypeface { get; }

		public ushort GlyphIndex { get; }

		public GlyphInfo(Typeface typeface, GlyphTypeface renderGlyphTypeface, GlyphTypeface originGlyphTypeface, char unicodeChar, ushort glyphIndex)
		{
			Typeface = typeface;
			RenderGlyphTypeface = renderGlyphTypeface;
			OriginGlyphTypeface = originGlyphTypeface;
			UnicodeChar = unicodeChar;
			GlyphIndex = glyphIndex;
		}
	}
}
