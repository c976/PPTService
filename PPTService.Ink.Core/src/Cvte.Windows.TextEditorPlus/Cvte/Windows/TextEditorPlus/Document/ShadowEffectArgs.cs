using System.Runtime.CompilerServices;
using System.Windows.Media;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class ShadowEffectArgs : ITextEffectArgs
	{
		public double Opacity { get; set; } = 1.0;

		public double ShadowDirection { get; set; }

		public double ShadowDistance { get; set; }

		public double ShadowOpacity { get; set; } = 1.0;

		public double ShadowBlur { get; set; }

		public Brush ShadowBrush { get; set; }

		public int Priority => 20;

		public override bool Equals(object obj)
		{
			ShadowEffectArgs shadowEffectArgs = obj as ShadowEffectArgs;
			if (shadowEffectArgs == null)
			{
				return false;
			}
			if (!Converter.AreEquals(ShadowDirection, shadowEffectArgs.ShadowDirection))
			{
				return false;
			}
			if (!Converter.AreEquals(ShadowDistance, shadowEffectArgs.ShadowDistance))
			{
				return false;
			}
			if (!Converter.AreEquals(ShadowOpacity, shadowEffectArgs.ShadowOpacity))
			{
				return false;
			}
			if (!Converter.AreEquals(ShadowBlur, shadowEffectArgs.ShadowBlur))
			{
				return false;
			}
			if (!Converter.AreEquals(ShadowBrush, shadowEffectArgs.ShadowBrush))
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
			return new ShadowEffectArgs
			{
				ShadowBlur = ShadowBlur,
				ShadowDirection = ShadowDirection,
				ShadowOpacity = ShadowOpacity,
				ShadowDistance = ShadowDistance,
				ShadowBrush = ShadowBrush
			};
		}

		public EffectSaveInfo ToSaveInfo()
		{
			return new ShadowEffectSaveInfo
			{
				Brush = ShadowBrush,
				Blur = ShadowBlur,
				Direction = ShadowDirection,
				Distance = ShadowDistance,
				Opacity = ShadowOpacity
			};
		}
	}
}
