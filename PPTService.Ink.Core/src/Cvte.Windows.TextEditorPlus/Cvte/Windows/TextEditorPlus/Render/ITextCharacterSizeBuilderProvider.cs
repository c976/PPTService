using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Render
{
	public interface ITextCharacterSizeBuilderProvider
	{
		Size GetTextSize(string text, string fontFamily, FontStyle fontStyle, FontWeight fontWeight, double fontSize);
	}
}
