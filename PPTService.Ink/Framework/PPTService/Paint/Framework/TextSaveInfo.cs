using PPTService.Paint.Framework.Controls.Texts;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.Standard;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTService.Paint.Framework
{
	[SaveInfoContract("Text", Description = "文本框")]
	public class TextSaveInfo : ElementSaveInfo
	{
		[SaveInfoMember("RichText", Description = "文本")]
		public RichTextSaveInfo RichText { get; set; }

		[SaveInfoMember("Fill", Description = "填充")]
		public Brush Fill { get; set; }

		[SaveInfoMember("BorderBrush", Description = "边框颜色")]
		public Brush BorderBrush { get; set; }

		[SaveInfoMember("BorderThickness", Description = "边框粗细")]
		public double BorderThickness { get; set; }

		[SaveInfoMember("BorderType", Description = "边框类型")]
		public BorderType BorderType { get; set; }
	}
}
