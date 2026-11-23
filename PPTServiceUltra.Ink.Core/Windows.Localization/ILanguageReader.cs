using System.Collections.Generic;
using System.Globalization;

namespace PPTServiceUltra.Ink.Core.Windows.Localization
{
	public interface ILanguageReader
	{
		Dictionary<string, string> Read(CultureInfo culture);
	}
}
