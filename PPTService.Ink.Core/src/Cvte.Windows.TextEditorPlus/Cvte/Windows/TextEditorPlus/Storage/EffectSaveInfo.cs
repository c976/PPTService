using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Storage
{
	public abstract class EffectSaveInfo : SaveInfo
	{
		public abstract ITextEffectArgs ToEffectArgs();
	}
}
