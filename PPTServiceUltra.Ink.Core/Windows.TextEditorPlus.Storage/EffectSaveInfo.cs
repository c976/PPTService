using PPTServiceUltra.Ink.Core.Storage;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Storage
{
	public abstract class EffectSaveInfo : SaveInfo
	{
		public abstract ITextEffectArgs ToEffectArgs();
	}
}
