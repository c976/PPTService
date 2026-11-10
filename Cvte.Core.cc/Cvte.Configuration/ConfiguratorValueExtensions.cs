using System;
using System.Globalization;
using System.Windows;

namespace Cvte.Configuration
{
	public static class ConfiguratorValueExtensions
	{
		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static Vector GetVector(this Configurator config, string key)
		{
			return Vector.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static Vector GetVector(this Configurator config, string key, Vector defaultValue)
		{
			string text = config.Get(key, string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			try
			{
				return Vector.Parse(text);
			}
			catch (InvalidOperationException)
			{
				return defaultValue;
			}
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetVector(this Configurator config, string key, Vector value)
		{
			config.Set(key, value.ToString());
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static Point GetPoint(this Configurator config, string key)
		{
			return Point.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static Point GetVector(this Configurator config, string key, Point defaultValue)
		{
			string text = config.Get(key, string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			try
			{
				return Point.Parse(text);
			}
			catch (InvalidOperationException)
			{
				return defaultValue;
			}
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetPoint(this Configurator config, string key, Point value)
		{
			config.Set(key, value.ToString());
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static Size GetSizet(this Configurator config, string key)
		{
			return Size.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static Size GetSize(this Configurator config, string key, Size defaultValue)
		{
			string text = config.Get(key, string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			try
			{
				return Size.Parse(text);
			}
			catch (InvalidOperationException)
			{
				return defaultValue;
			}
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetSize(this Configurator config, string key, Size value)
		{
			config.Set(key, value.ToString(CultureInfo.InvariantCulture));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static Rect GetRect(this Configurator config, string key)
		{
			return Rect.Parse(config.Get(key));
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 GetAsync<T> 替代。")]
		public static Rect GetRect(this Configurator config, string key, Rect defaultValue)
		{
			string text = config.Get(key, string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				return defaultValue;
			}
			try
			{
				return Rect.Parse(text);
			}
			catch (InvalidOperationException)
			{
				return defaultValue;
			}
		}

		[Obsolete("同步读写配置值的方法将于 2017年6月18日 删除；请使用 SetAsync<T> 替代。")]
		public static void SetRect(this Configurator config, string key, Rect value)
		{
			config.Set(key, value.ToString(CultureInfo.InvariantCulture));
		}
	}
}
