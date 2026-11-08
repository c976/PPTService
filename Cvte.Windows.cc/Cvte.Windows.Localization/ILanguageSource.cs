using System.Collections.Generic;

namespace Cvte.Windows.Localization
{
	public interface ILanguageSource
	{
		IEnumerable<Language> GetLanguages();
	}
}
