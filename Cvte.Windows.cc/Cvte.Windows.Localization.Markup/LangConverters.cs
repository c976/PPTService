using System.Globalization;
using System.Windows.Media;

namespace Cvte.Windows.Localization.Markup
{
	public static class LangConverters
	{
		public static object Int32Converter(string langKey, string langValue, CultureInfo culture)
		{
			int result;
			int.TryParse(langValue, NumberStyles.Integer, culture, out result);
			return result;
		}

		public static object DoubleConverter(string langKey, string langValue, CultureInfo culture)
		{
			double result;
			double.TryParse(langValue, NumberStyles.Any, culture, out result);
			return result;
		}

		public static object FontFamilyConverter(string langKey, string langValue, CultureInfo culture)
		{
			return new FontFamily(langValue);
		}
	}
}
