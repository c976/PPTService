using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PPTServiceUltra.Ink.Core.Windows.Localization
{
	public class DictionaryLanguageSource : ILanguageSource, ILanguageReader
	{
		private readonly Dictionary<CultureInfo, Dictionary<string, string>> _languageDictionary = new Dictionary<CultureInfo, Dictionary<string, string>>();

		public Dictionary<string, string> this[CultureInfo culture]
		{
			get
			{
				return Read(culture);
			}
			set
			{
				_languageDictionary[culture] = value;
			}
		}

		IEnumerable<Language> ILanguageSource.GetLanguages()
		{
			return _languageDictionary.Select((KeyValuePair<CultureInfo, Dictionary<string, string>> pair) => new Language(pair.Key, this));
		}

		public Dictionary<string, string> Read(CultureInfo culture)
		{
			Dictionary<string, string> value;
			if (_languageDictionary.TryGetValue(culture, out value))
			{
				return value;
			}
			throw new CultureNotFoundException("语言字典中不包含指定的区域属性语言项。");
		}
	}
}
