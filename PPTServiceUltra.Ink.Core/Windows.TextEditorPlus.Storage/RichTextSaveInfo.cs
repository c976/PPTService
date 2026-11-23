using System.Collections.Generic;
using System.Windows;
using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("RichText")]
	public class RichTextSaveInfo : SaveInfo
	{
		[SaveInfoMember("SizeToContent")]
		public SizeToContent SizeToContent { get; set; }

		[SaveInfoMember("ArrangingType", DefaultValue = ArrangingType.Horizontal)]
		public ArrangingType ArrangingType { get; set; }

		[SaveInfoMember("VerticalTextAlignment")]
		public VerticalAlignment VerticalTextAlignment { get; set; }

		[SaveInfoMember("TextLines")]
		public List<ParagraphSaveInfo> Paragraphs { get; set; } = new List<ParagraphSaveInfo>();

		[SaveInfoMember("Text", Description = "纯文本")]
		public string Text { get; set; }
	}
}
