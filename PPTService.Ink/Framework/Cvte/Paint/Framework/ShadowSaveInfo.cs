using Cvte.Storage;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Framework
{
	[SaveInfoContract("Shadow", Description = "阴影")]
	public abstract class ShadowSaveInfo : EffectSaveInfo
	{
		[SaveInfoMember("ShadowBrush", Description = "阴影填充")]
		public Brush ShadowBrush { get; set; }

		[SaveInfoMember("Opacity", Description = "阴影透明度")]
		public double Opacity { get; set; }

		[SaveInfoMember("BlurRadius", Description = "阴影模糊半径")]
		public double BlurRadius { get; set; }
	}
}
