using System.Windows;
using System.Windows.Media.TextFormatting;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus
{
	internal class MongolianTextLineProperties : TextParagraphProperties
	{
		public double SetIndent { get; set; }

		public double SetLineHeight { get; set; }

		public TextAlignment SetTextAlignment { get; set; }

		public TextRunProperties SetDefaultTextRunProperties { get; set; }

		public override bool FirstLineInParagraph => false;

		public override double Indent => SetIndent;

		public override double LineHeight => SetLineHeight;

		public override TextWrapping TextWrapping => TextWrapping.Wrap;

		public override FlowDirection FlowDirection => FlowDirection.LeftToRight;

		public override TextAlignment TextAlignment => SetTextAlignment;

		public override TextRunProperties DefaultTextRunProperties => SetDefaultTextRunProperties;

		public override TextMarkerProperties TextMarkerProperties => null;
	}
}
