using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("AutoNumber", Description = "自动项目符号")]
	public class AutoNumberSaveInfo : SaveInfo
	{
		[SaveInfoMember("AutoNumberType", Description = "数字类型")]
		public AutoNumberType AutoNumberType { get; set; }

		[SaveInfoMember("StartAt", Description = "起始数字")]
		public int StartAt { get; set; }
	}
}
