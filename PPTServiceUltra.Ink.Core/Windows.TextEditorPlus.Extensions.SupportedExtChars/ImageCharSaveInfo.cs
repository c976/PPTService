using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Storage.Standard;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions.SupportedExtChars
{
	[SaveInfoContract("ImageCharSaveInfo", Description = "图片扩展字符的存储，用于放在文本框里面将图片当成字符")]
	[SaveInfoExtension("ImageCharSaveInfo", typeof(RunSaveInfo))]
	public class ImageCharSaveInfo : ExtCharSaveInfo
	{
		[SaveInfoMember("Source", IsRequired = true, Description = "图片源")]
		public Uri Source { get; set; }

		[SaveInfoMember("LogicalCenter", Description = "图片的逻辑中心点")]
		public override Point LogicalCenter { get; set; }

		[SaveInfoMember("Size", Description = "图片的大小")]
		public override Size Size { get; set; }
	}
}
