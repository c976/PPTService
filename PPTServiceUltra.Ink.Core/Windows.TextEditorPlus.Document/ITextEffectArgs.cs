using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public interface ITextEffectArgs
	{
		double Opacity { get; set; }

		int Priority { get; }

		ITextEffectArgs Clone();

		EffectSaveInfo ToSaveInfo();
	}
}
