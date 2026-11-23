using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Storage.Standard;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage
{
	[SaveInfoContract("TextFill", Description = "填充")]
	public class FillEffectSaveInfo : EffectSaveInfo
	{
		[SaveInfoMember("FillBrush", Description = "画刷")]
		public Brush FillBrush { get; set; }

		public override bool Equals(object obj)
		{
			FillEffectSaveInfo fillEffectSaveInfo = obj as FillEffectSaveInfo;
			if (fillEffectSaveInfo == null)
			{
				return false;
			}
			if (!Converter.AreEquals(FillBrush, fillEffectSaveInfo.FillBrush))
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
			return new FillEffectArgs
			{
				FillBrush = FillBrush
			};
		}
	}
}
