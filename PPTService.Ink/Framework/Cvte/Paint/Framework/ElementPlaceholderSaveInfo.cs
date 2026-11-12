using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	[SaveInfoExtension("ElementPlaceholder", typeof(TextSaveInfo))]
	[SaveInfoContract("ElementPlaceholder", Description = "元素占位符")]
	public class ElementPlaceholderSaveInfo : SaveInfo
	{
		[SaveInfoMember("Type", Description = "元素占位符类型")]
		public PlaceholderType Type { get; set; }

		[SaveInfoMember("Tips", Description = "元素占位符内容")]
		public string Tips { get; set; }
	}
}
