using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Paint.Inks.Storage
{
	[SaveInfoContract("Ink")]
	public class HardPenInkSaveInfo : InkSaveInfo
	{
		[SaveInfoMember("StylusTipTransform")]
		public Matrix StylusTipTransform { get; set; }

		[SaveInfoMember("FitToCurve")]
		public bool FitToCurve { get; set; }

		public HardPenInkSaveInfo()
		{
			Type = "HardPenStroke";
		}
	}
}
