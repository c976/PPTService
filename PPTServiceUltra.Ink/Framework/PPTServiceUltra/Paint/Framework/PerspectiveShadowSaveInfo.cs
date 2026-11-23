using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Paint.Framework
{
	[SaveInfoContract("PerspectiveShadow", Description = "透视阴影")]
	public class PerspectiveShadowSaveInfo : ShadowSaveInfo
	{
		[SaveInfoMember("OffsetY", Description = "Y轴偏移")]
		public double OffsetY { get; set; }

		[SaveInfoMember("ShadowMatrix", Description = "映射变换矩阵")]
		public Matrix ShadowMatrix { get; set; }
	}
}
