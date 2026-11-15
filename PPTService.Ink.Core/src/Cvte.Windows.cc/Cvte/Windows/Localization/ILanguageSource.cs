using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.Localization
{
	public interface ILanguageSource
	{
		IEnumerable<Language> GetLanguages();
	}
}
