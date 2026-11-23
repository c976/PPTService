using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Paint.Framework
{
	[SaveInfoContract("Reflection", Description = "倒影")]
	public class ReflectionSaveInfo : EffectSaveInfo
	{
		[SaveInfoMember("OffsetY", Description = "Y轴偏移")]
		public double OffsetY { get; set; }

		[SaveInfoMember("Opacity", Description = "透明度")]
		public double Opacity { get; set; }

		[SaveInfoMember("OpacityMask", Description = "渐变画刷")]
		public Brush OpacityMask { get; set; }
	}
}
