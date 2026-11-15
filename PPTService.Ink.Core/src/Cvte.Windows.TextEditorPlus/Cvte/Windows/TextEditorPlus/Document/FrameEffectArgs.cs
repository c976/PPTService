using System.Runtime.CompilerServices;
using System.Windows.Media;
using PPTService.Ink.Core.Storage.Standard;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class FrameEffectArgs : ITextEffectArgs
	{
		public double FrameOpacity { get; set; } = 1.0;

		public double Opacity { get; set; } = 1.0;

		public double FrameThinkness { get; set; }

		public System.Windows.Media.Brush FrameBrush { get; set; }

		public int Priority => 40;

		public override bool Equals(object obj)
		{
			FrameEffectArgs frameEffectArgs = obj as FrameEffectArgs;
			if (frameEffectArgs == null)
			{
				return false;
			}
			if (!Converter.AreEquals(FrameOpacity, frameEffectArgs.FrameOpacity))
			{
				return false;
			}
			if (!Converter.AreEquals(FrameThinkness, frameEffectArgs.FrameThinkness))
			{
				return false;
			}
			if (!Converter.AreEquals(FrameBrush, frameEffectArgs.FrameBrush))
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
			return new FrameEffectArgs
			{
				FrameOpacity = FrameOpacity,
				FrameThinkness = FrameThinkness,
				FrameBrush = (PPTService.Ink.Core.Storage.Standard.Brush)FrameBrush
			};
		}

		public EffectSaveInfo ToSaveInfo()
		{
			return new FrameEffectSaveInfo
			{
				FrameBrush = FrameBrush,
				FrameThinkness = FrameThinkness,
				FrameOpacity = FrameOpacity
			};
		}
	}
}
