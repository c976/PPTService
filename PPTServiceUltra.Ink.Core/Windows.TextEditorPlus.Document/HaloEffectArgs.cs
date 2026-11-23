using System.Runtime.CompilerServices;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class HaloEffectArgs : ITextEffectArgs
	{
		public double HaloWeight { get; set; }

		public double Opacity { get; set; } = 1.0;

		public double HaloOpacity { get; set; } = 1.0;

		public Color HaloColor { get; set; }

		public int Priority => 10;

		public override bool Equals(object obj)
		{
			HaloEffectArgs haloEffectArgs = obj as HaloEffectArgs;
			if (haloEffectArgs == null)
			{
				return false;
			}
			if (!HaloWeight.Equals(haloEffectArgs.HaloWeight))
			{
				return false;
			}
			if (!HaloOpacity.Equals(haloEffectArgs.HaloOpacity))
			{
				return false;
			}
			if (!HaloColor.Equals(haloEffectArgs.HaloColor))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public ITextEffectArgs Clone()
		{
			return new HaloEffectArgs
			{
				HaloWeight = HaloWeight,
				HaloOpacity = HaloOpacity,
				HaloColor = HaloColor
			};
		}

		public EffectSaveInfo ToSaveInfo()
		{
			return new HaloEffectSaveInfo
			{
				Color = HaloColor,
				Opacity = HaloOpacity,
				Weight = HaloWeight
			};
		}
	}
}
