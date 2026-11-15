using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.Standard;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("ExtCharSaveInfo", Description = "文本扩展字符")]
	public abstract class ExtCharSaveInfo : SaveInfo
	{
		[SaveInfoMember("LogicalCenter", Description = "图片的逻辑中心点")]
		public abstract Point LogicalCenter { get; set; }

		[SaveInfoMember("Size", Description = "图片的大小")]
		public abstract Size Size { get; set; }
	}
}
