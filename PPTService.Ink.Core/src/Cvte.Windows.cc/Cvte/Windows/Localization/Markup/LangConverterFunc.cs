using System.Globalization;

namespace PPTService.Ink.Core.Windows.Localization.Markup
{
	public delegate object LangConverterFunc(string langKey, string langValue, CultureInfo culture);
}
