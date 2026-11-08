using System.Collections.Generic;
using System.Globalization;

namespace Cvte.Windows.Localization
{
	public interface ILanguageReader
	{
		Dictionary<string, string> Read(CultureInfo culture);
	}
}
