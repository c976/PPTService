using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	[SaveInfoContract("DropShadow", Description = "投影")]
	public class DropShadowSaveInfo : ShadowSaveInfo
	{
		[SaveInfoMember("Offset", Description = "投影的偏移量")]
		public double Offset { get; set; }

		[SaveInfoMember("Angle", Description = "投影的角度")]
		public double Angle { get; set; }
	}
}
