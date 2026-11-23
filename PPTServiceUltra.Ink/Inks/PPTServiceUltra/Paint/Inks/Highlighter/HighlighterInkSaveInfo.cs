using PPTServiceUltra.Paint.Inks.Storage;
using PPTServiceUltra.Ink.Core.Storage;

namespace PPTServiceUltra.Paint.Inks.Highlighter
{
	public class HighlighterInkSaveInfo : InkSaveInfo
	{
		[SaveInfoMember("HighlighterOpacity", Description = "荧光笔透明度", IsRuntime = true)]
		public double HighlighterOpacity { get; set; }
	}
}
