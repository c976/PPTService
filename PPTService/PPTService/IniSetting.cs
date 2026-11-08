using System;
using System.Globalization;

namespace PPTService
{
	public abstract class IniSetting
	{
		private readonly string _section;

		private readonly string _settingPath;

		protected IniSetting(string settingPath, string section)
		{
			_settingPath = settingPath;
			_section = section;
		}

		public T GetSettingValue<T>(string key, T defaultValue, Func<string, T, T> convertFunc, string section, string path)
		{
			string text = IniUtility.Read(section, key, _settingPath);
			if (!string.IsNullOrEmpty(text))
			{
				return convertFunc(text, defaultValue);
			}
			return defaultValue;
		}

		public string GetSettingValue(string key, string defaultValue, string section, string path)
		{
			return GetSettingValue(key, defaultValue, (string valueToConvert, string defaultValueToConvert) => (!string.IsNullOrEmpty(valueToConvert)) ? valueToConvert : defaultValueToConvert, section, path);
		}

		public string GetSettingValue(string key, string defaultValue, string section)
		{
			return GetSettingValue(key, defaultValue, section, _settingPath);
		}

		public string GetSettingValue(string key, string defaultValue)
		{
			return GetSettingValue(key, defaultValue, _section, _settingPath);
		}

		public int GetSettingValue(string key, int defaultValue, string section, string path)
		{
			return GetSettingValue(key, defaultValue, (string valueToConvert, int defaultValueToConvert) =>
			{
				int result;
				return (!int.TryParse(valueToConvert, out result)) ? defaultValueToConvert : result;
			}, section, path);
		}

		public int GetSettingValue(string key, int defaultValue, string section)
		{
			return GetSettingValue(key, defaultValue, section, _settingPath);
		}

		public int GetSettingValue(string key, int defaultValue)
		{
			return GetSettingValue(key, defaultValue, _section, _settingPath);
		}

		public bool GetSettingValue(string key, bool defaultValue, string section, string path)
		{
			return GetSettingValue(key, defaultValue, (string valueToConvert, bool defaultValueToConvert) =>
			{
				int result;
				return (!int.TryParse(valueToConvert, out result)) ? defaultValueToConvert : (1 == result);
			}, section, path);
		}

		public bool GetSettingValue(string key, bool defaultValue, string section)
		{
			return GetSettingValue(key, defaultValue, section, _settingPath);
		}

		public bool GetSettingValue(string key, bool defaultValue)
		{
			return GetSettingValue(key, defaultValue, _section, _settingPath);
		}

		public void SetSettingValue<T>(string key, T value, Func<T, string> convertFunc, string section, string path)
		{
			IniUtility.Write(section, key, convertFunc(value), _settingPath);
		}

		public void SetSettingValue(string key, string value, string section, string path)
		{
			SetSettingValue(key, value, (string valueToConvert) => valueToConvert, section, path);
		}

		public void SetSettingValue(string key, string value, string section)
		{
			SetSettingValue(key, value, section, _settingPath);
		}

		public void SetSettingValue(string key, string value)
		{
			SetSettingValue(key, value, _section, _settingPath);
		}

		public void SetSettingValue(string key, int value, string section, string path)
		{
			SetSettingValue(key, value, (int valueToConvert) => valueToConvert.ToString(CultureInfo.InvariantCulture), section, path);
		}

		public void SetSettingValue(string key, int value, string section)
		{
			SetSettingValue(key, value, section, _settingPath);
		}

		public void SetSettingValue(string key, int value)
		{
			SetSettingValue(key, value, _section, _settingPath);
		}

		public void SetSettingValue(string key, bool value, string section, string path)
		{
			SetSettingValue(key, value, (bool valueToConvert) => (!valueToConvert) ? "0" : "1", section, path);
		}

		public void SetSettingValue(string key, bool value, string section)
		{
			SetSettingValue(key, value, section, _settingPath);
		}

		public void SetSettingValue(string key, bool value)
		{
			SetSettingValue(key, value, _section, _settingPath);
		}
	}
}
