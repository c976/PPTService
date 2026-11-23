using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.Localization
{
	internal class ResourceDictionaryLanguageReader : ILanguageReader
	{
		public ResourceDictionary XamlDictionary { get; set; }

		public ResourceDictionaryLanguageReader(ResourceDictionary dictionary)
		{
			XamlDictionary = dictionary;
		}

		public Dictionary<string, string> Read(CultureInfo culture)
		{
			return XamlDictionary.OfType<DictionaryEntry>().ToDictionary((DictionaryEntry x) => x.Key.ToString(), (DictionaryEntry x) => string.Format(culture, "{0}", new object[1] { x.Value }));
		}
	}
}
