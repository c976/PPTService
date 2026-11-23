using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Storage.Standard;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("TextHalo", Description = "发光")]
	public class HaloEffectSaveInfo : EffectSaveInfo
	{
		[SaveInfoMember("Opacity", Description = "不透明度", Aliases = new string[] { "HaloOpacity" })]
		public double Opacity { get; set; }

		[SaveInfoMember("Weight", Description = "范围", Aliases = new string[] { "HaloWeight" })]
		public double Weight { get; set; }

		[SaveInfoMember("Color", Description = "颜色", Aliases = new string[] { "HaloColor" })]
		public Color Color { get; set; }

		public override bool Equals(object obj)
		{
			HaloEffectSaveInfo haloEffectSaveInfo = obj as HaloEffectSaveInfo;
			if (haloEffectSaveInfo == null)
			{
				return false;
			}
			if (!Weight.Equals(haloEffectSaveInfo.Weight))
			{
				return false;
			}
			if (!Opacity.Equals(haloEffectSaveInfo.Opacity))
			{
				return false;
			}
			if (!Color.Equals(haloEffectSaveInfo.Color))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public override ITextEffectArgs ToEffectArgs()
		{
			if (Opacity < 0.0)
			{
				Opacity = 0.0;
			}
			if (Opacity > 1.0)
			{
				Opacity = 1.0;
			}
			if (Weight > 500.0)
			{
				Weight = 500.0;
			}
			if (Weight < 0.0)
			{
				Weight = 0.0;
			}
			return new HaloEffectArgs
			{
				HaloColor = Color,
				HaloOpacity = Opacity,
				HaloWeight = Weight
			};
		}
	}
}
