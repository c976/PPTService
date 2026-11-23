using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Storage.Standard;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("TextFrame", Description = "边框")]
	public class FrameEffectSaveInfo : EffectSaveInfo
	{
		[SaveInfoMember("FrameOpacity", Description = "透明度")]
		public double FrameOpacity { get; set; }

		[SaveInfoMember("FrameThinkness", Description = "宽度")]
		public double FrameThinkness { get; set; }

		[SaveInfoMember("FrameBrush", Description = "画刷")]
		public Brush FrameBrush { get; set; }

		public override bool Equals(object obj)
		{
			FrameEffectSaveInfo frameEffectSaveInfo = obj as FrameEffectSaveInfo;
			if (frameEffectSaveInfo == null)
			{
				return false;
			}
			if (!Converter.AreEquals(FrameOpacity, frameEffectSaveInfo.FrameOpacity))
			{
				return false;
			}
			if (!Converter.AreEquals(FrameThinkness, frameEffectSaveInfo.FrameThinkness))
			{
				return false;
			}
			if (!Converter.AreEquals(FrameBrush, frameEffectSaveInfo.FrameBrush))
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
			if (FrameThinkness > 400.0)
			{
				FrameThinkness = 400.0;
			}
			if (FrameThinkness < 0.0)
			{
				FrameThinkness = 0.0;
			}
			if (FrameOpacity < 0.0)
			{
				FrameOpacity = 0.0;
			}
			if (FrameOpacity > 1.0)
			{
				FrameOpacity = 1.0;
			}
			return new FrameEffectArgs
			{
				FrameBrush = FrameBrush,
				FrameThinkness = FrameThinkness,
				FrameOpacity = FrameOpacity
			};
		}
	}
}
