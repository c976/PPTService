using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils.Glyph;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	public class TextCharGlyphData
	{
		public double FontSize { get; set; }

		public GlyphRun GlyphRun { get; set; }

		public GlyphInfo GlyphInfo { get; set; }
	}
}
