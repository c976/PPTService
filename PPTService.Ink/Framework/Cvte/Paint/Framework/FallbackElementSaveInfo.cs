using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	[SaveInfoContract("Fallback", Description = "当元素无法解析时的回退元素", IsFallbackElement = true)]
	public class FallbackElementSaveInfo : ElementSaveInfo
	{
	}
}
