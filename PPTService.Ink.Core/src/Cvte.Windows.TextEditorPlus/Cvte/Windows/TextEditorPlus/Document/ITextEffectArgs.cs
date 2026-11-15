using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public interface ITextEffectArgs
	{
		double Opacity { get; set; }

		int Priority { get; }

		ITextEffectArgs Clone();

		EffectSaveInfo ToSaveInfo();
	}
}
