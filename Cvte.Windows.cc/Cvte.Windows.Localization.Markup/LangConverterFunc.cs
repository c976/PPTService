using System.Globalization;

namespace Cvte.Windows.Localization.Markup
{
	public delegate object LangConverterFunc(string langKey, string langValue, CultureInfo culture);
}
