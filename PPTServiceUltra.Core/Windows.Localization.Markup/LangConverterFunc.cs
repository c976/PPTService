using System.Globalization;

namespace PPTServiceUltra.Core.Windows.Localization.Markup
{
	public delegate object LangConverterFunc(string langKey, string langValue, CultureInfo culture);
}
