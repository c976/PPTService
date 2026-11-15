using System.Windows.Media;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class LineSpaceHelper
	{
		public static double CalcRenderLineSpace(RunProperty runProperty, double lineSpace)
		{
			double fontSize = runProperty.FontSize;
			FontFamily fontFamily = runProperty.FontFamily;
			return CalcRenderLineSpace(lineSpace, fontSize, fontFamily);
		}

		private static double CalcRenderLineSpace(double lineSpace, double fontSize, FontFamily fontFamily)
		{
			double lineSpacing = fontFamily.LineSpacing;
			return CalcRenderLineSpace(lineSpace, fontSize, lineSpacing);
		}

		private static double CalcRenderLineSpace(double lineSpace, double fontSize, double fontFamilyLineSpacing)
		{
			return fontSize * fontFamilyLineSpacing * (lineSpace - 1.0) / 10.0;
		}

		[PublicAPI]
		public static double PptLineSpacingToTextEditorPlusLineSpacing(double pptOriginLineSpacing, double fontFamilyLineSpacing)
		{
			return (1.2018 * pptOriginLineSpacing + 0.0034 - fontFamilyLineSpacing) * 10.0 / fontFamilyLineSpacing + 1.0;
		}
	}
}
