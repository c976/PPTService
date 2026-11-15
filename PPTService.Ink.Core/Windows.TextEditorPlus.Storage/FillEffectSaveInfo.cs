using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.Standard;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
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
