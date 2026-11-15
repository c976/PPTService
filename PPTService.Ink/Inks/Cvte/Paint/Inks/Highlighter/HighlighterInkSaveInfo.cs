using Cvte.Paint.Inks.Storage;
using PPTService.Ink.Core.Storage;

namespace Cvte.Paint.Inks.Highlighter
{
	public class HighlighterInkSaveInfo : InkSaveInfo
	{
		[SaveInfoMember("HighlighterOpacity", Description = "荧光笔透明度", IsRuntime = true)]
		public double HighlighterOpacity { get; set; }
	}
}
