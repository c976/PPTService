using System;
using System.Globalization;

namespace Cvte.Configuration
{
	public static class ConfiguratorValueExtension
	{
		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private static readonly string DoubleNaN = double.NaN.ToString(CultureInfo.InvariantCulture);

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private static readonly string DoublePositiveInfinity = double.PositiveInfinity.ToString(CultureInfo.InvariantCulture);

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private static readonly string DoubleNegativeInfinity = double.NegativeInfinity.ToString(CultureInfo.InvariantCulture);

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static string GetString(this Configurator config, string key)
		{
			return config.Get(key);
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static string GetString(this Configurator config, string key, string defaultValue)
		{
			return config.Get(key, defaultValue);
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetString(this Configurator config, string key, string value)
		{
			config.Set(key, value);
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static int GetInt32(this Configurator config, string key)
		{
			return int.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static int GetInt32(this Configurator config, string key, int defaultValue)
		{
			int result;
			if (!int.TryParse(config.Get(key, string.Empty), out result))
			{
				return defaultValue;
			}
			return result;
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetInt32(this Configurator config, string key, int value)
		{
			config.Set(key, value.ToString(CultureInfo.InvariantCulture));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static long GetInt64(this Configurator config, string key)
		{
			return int.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static long GetInt64(this Configurator config, string key, long defaultValue)
		{
			long result;
			if (!long.TryParse(config.Get(key, string.Empty), out result))
			{
				return defaultValue;
			}
			return result;
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetInt64(this Configurator config, string key, long value)
		{
			config.Set(key, value.ToString(CultureInfo.InvariantCulture));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static char GetChar(this Configurator config, string key)
		{
			return char.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static char GetChar(this Configurator config, string key, char defaultValue)
		{
			char result;
			if (!char.TryParse(config.Get(key, string.Empty), out result))
			{
				return defaultValue;
			}
			return result;
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetChar(this Configurator config, string key, char value)
		{
			config.Set(key, value.ToString(CultureInfo.InvariantCulture));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static float GetSingle(this Configurator config, string key)
		{
			return float.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static float GetSingle(this Configurator config, string key, float defaultValue)
		{
			float result;
			if (!float.TryParse(config.Get(key, string.Empty), out result))
			{
				return defaultValue;
			}
			return result;
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetSingle(this Configurator config, string key, float value)
		{
			config.Set(key, value.ToString(CultureInfo.InvariantCulture));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static double GetDouble(this Configurator config, string key)
		{
			return ParseDouble(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static double GetDouble(this Configurator config, string key, double defaultValue)
		{
			double result;
			if (!TryParseDouble(config.Get(key, string.Empty), out result))
			{
				return defaultValue;
			}
			return result;
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private static double ParseDouble(string s)
		{
			double result;
			if (!TryParseSpecialDouble(s, out result))
			{
				return double.Parse(s);
			}
			return result;
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private static bool TryParseDouble(string s, out double result)
		{
			if (!TryParseSpecialDouble(s, out result))
			{
				return double.TryParse(s, out result);
			}
			return true;
		}

		[Obsolete("同步读写配置的方法将于 2017年6月18日 删除；请在方法名称后加上 Async 替代。")]
		private unsafe static bool TryParseSpecialDouble(string s, out double result)
		{
			void* ptr = stackalloc byte[9];
			((sbyte*)ptr)[8] = 1;
			*(double*)ptr = 0.0;
			if (s.Equals(DoubleNaN))
			{
				*(double*)ptr = double.NaN;
			}
			else if (s.Equals(DoublePositiveInfinity))
			{
				*(double*)ptr = double.PositiveInfinity;
			}
			else if (s.Equals(DoubleNegativeInfinity))
			{
				*(double*)ptr = double.NegativeInfinity;
			}
			else
			{
				((sbyte*)ptr)[8] = 0;
			}
			result = *(double*)ptr;
			return ((bool*)ptr)[8];
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetDouble(this Configurator config, string key, double value)
		{
			config.Set(key, value.ToString(CultureInfo.InvariantCulture));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static bool GetBoolean(this Configurator config, string key)
		{
			return bool.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static bool GetBoolean(this Configurator config, string key, bool defaultValue)
		{
			bool result;
			if (!bool.TryParse(config.Get(key, string.Empty), out result))
			{
				return defaultValue;
			}
			return result;
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetBoolean(this Configurator config, string key, bool value)
		{
			config.Set(key, value.ToString());
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static decimal GetDecimal(this Configurator config, string key)
		{
			return decimal.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static decimal GetDecimal(this Configurator config, string key, decimal defaultValue)
		{
			decimal result;
			if (!decimal.TryParse(config.Get(key, string.Empty), out result))
			{
				return defaultValue;
			}
			return result;
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetDecimal(this Configurator config, string key, decimal value)
		{
			config.Set(key, value.ToString(CultureInfo.InvariantCulture));
		}
	}
}
