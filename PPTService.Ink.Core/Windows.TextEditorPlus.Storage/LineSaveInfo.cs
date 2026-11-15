using PPTService.Ink.Core.Storage;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("LineProperty")]
	public class LineSaveInfo : SaveInfo
	{
		[SaveInfoMember("StartOffset", Description = "相对于段落的行起始偏移量")]
		public int StartOffset { get; set; }

		[SaveInfoMember("Length", Description = "行包括的字符数量")]
		public int Length { get; set; }
	}
}
