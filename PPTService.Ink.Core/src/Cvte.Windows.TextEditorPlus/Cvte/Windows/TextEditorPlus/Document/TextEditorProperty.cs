using System.Windows;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class TextEditorProperty
	{
		public RunProperty RunProperty { get; set; }

		public ParagraphProperty ParagraphProperty { get; set; }

		public VerticalAlignment? TextVerticalAlignment { get; set; }

		public ArrangingType? ArrangingType { get; set; }
	}
}
