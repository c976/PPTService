using System.Collections.Generic;
using System.Globalization;

namespace PPTService.Ink.Core.Windows.Localization
{
	public interface ILanguageReader
	{
		Dictionary<string, string> Read(CultureInfo culture);
	}
}
