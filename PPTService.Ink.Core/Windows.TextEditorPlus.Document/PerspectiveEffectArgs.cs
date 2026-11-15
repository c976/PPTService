using System.Runtime.CompilerServices;
using System.Windows.Media;
using PPTService.Ink.Core.Storage.Standard;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class PerspectiveEffectArgs : ITextEffectArgs
	{
		public double Opacity { get; set; } = 1.0;

		public double PerspectiveAngle { get; set; }

		public double PerspectiveDepth { get; set; }

		public double PerspectiveOpacity { get; set; } = 1.0;

		public double PerspectiveBlur { get; set; }

		public System.Windows.Media.Brush PerspectiveBrush { get; set; }

		public int Priority => 30;

		public override bool Equals(object obj)
		{
			PerspectiveEffectArgs perspectiveEffectArgs = obj as PerspectiveEffectArgs;
			if (perspectiveEffectArgs == null)
			{
				return false;
			}
			if (!Converter.AreEquals(PerspectiveAngle, perspectiveEffectArgs.PerspectiveAngle))
			{
				return false;
			}
			if (!Converter.AreEquals(PerspectiveDepth, perspectiveEffectArgs.PerspectiveDepth))
			{
				return false;
			}
			if (!Converter.AreEquals(PerspectiveOpacity, perspectiveEffectArgs.PerspectiveOpacity))
			{
				return false;
			}
			if (!Converter.AreEquals(PerspectiveBlur, perspectiveEffectArgs.PerspectiveBlur))
			{
				return false;
			}
			if (!Converter.AreEquals(PerspectiveBrush, perspectiveEffectArgs.PerspectiveBrush))
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
			return new PerspectiveEffectArgs
			{
				PerspectiveAngle = PerspectiveAngle,
				PerspectiveDepth = PerspectiveDepth,
				PerspectiveOpacity = PerspectiveOpacity,
				PerspectiveBlur = PerspectiveBlur,
				PerspectiveBrush = (PPTService.Ink.Core.Storage.Standard.Brush)PerspectiveBrush
			};
		}

		public EffectSaveInfo ToSaveInfo()
		{
			return new PerspectiveEffectSaveInfo
			{
				Brush = PerspectiveBrush,
				Angle = PerspectiveAngle,
				Blur = PerspectiveBlur,
				Depth = PerspectiveDepth,
				Opacity = PerspectiveOpacity
			};
		}
	}
}
