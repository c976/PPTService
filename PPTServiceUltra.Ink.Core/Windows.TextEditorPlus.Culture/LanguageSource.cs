using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PPTServiceUltra.Ink.Core.Windows.Localization;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Culture
{
	public class LanguageSource : ILanguageSource, ILanguageReader
	{
		private readonly ILanguage[] _languageList = new ILanguage[3]
		{
			new ZhCHSLanguage(),
			new ZhCHTLanguage(),
			new EnUSLanguage()
		};

		public IEnumerable<Language> GetLanguages()
		{
			return _languageList.Select((ILanguage temp) => new Language(temp.Culture, this));
		}

		public Dictionary<string, string> Read(CultureInfo culture)
		{
			return _languageList.FirstOrDefault((ILanguage temp) => object.Equals(temp.Culture.Name, culture.Name))?.Language;
		}
	}
}
