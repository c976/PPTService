using System.Collections.Generic;

namespace PPTServiceUltra.Ink.Core.Windows.Localization
{
	public interface ILanguageSource
	{
		IEnumerable<Language> GetLanguages();
	}
}
