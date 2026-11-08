using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media;

namespace Cvte.Configuration
{
	public static class ConfiguratorArrayExtensions
	{
		public static int[] GetInt32Array(this Configurator config, string key)
		{
			return config.GetArray(key).Where((string x) =>
			{
				int temp;
				return int.TryParse(x, out temp);
			}).Select(int.Parse)
				.ToArray();
		}

		public static void SetInt32Array(this Configurator config, string key, IEnumerable<int> value)
		{
			config.SetArray(key, value.Select((int x) => x.ToString(CultureInfo.InvariantCulture)));
		}

		public static Color[] GetColorArray(this Configurator config, string key)
		{
			return (from convertFromString in config.GetArray(key).Select(ColorConverter.ConvertFromString)
				where convertFromString != null
				select convertFromString).Cast<Color>().ToArray();
		}

		public static FontFamily[] GetFontFamilyArray(this Configurator config, string key)
		{
			return (from x in config.GetArray(key)
				select new FontFamily(x)).ToArray();
		}

		public static void SetColorArray(this Configurator config, string key, IEnumerable<Color> value)
		{
			config.SetArray(key, value.Select((Color x) => x.ToString(CultureInfo.InvariantCulture)));
		}
	}
}
