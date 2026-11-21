using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace PPTService
{
	/// <summary>
	/// 运行时设置存储：%APPDATA%\PPTServiceUltra\RuntimeConfig.json，取代旧的 app.config
	/// （INI 格式 + kernel32 WritePrivateProfileString）。值统一以字符串保存，bool 为 "1"/"0"，
	/// 保持与旧配置完全一致的读写语义；ConfigController 只需把基类从 IniSetting 换成本类。
	/// 读写都加锁并即时落盘，避免与 UI 线程的多次写入互相覆盖。
	/// </summary>
	public abstract class JsonSetting
	{
		private readonly string _section;
		private readonly string _settingPath;
		private readonly object _syncRoot = new object();
		private Dictionary<string, string> _values;

		protected JsonSetting(string settingPath, string section)
		{
			_settingPath = settingPath;
			_section = section;
		}

		private static string MakeKey(string section, string key)
		{
			return string.IsNullOrEmpty(section) ? key : section + "." + key;
		}

		private void EnsureLoaded()
		{
			if (_values != null)
			{
				return;
			}
			_values = new Dictionary<string, string>(StringComparer.Ordinal);
			try
			{
				if (File.Exists(_settingPath))
				{
					Dictionary<string, string> loaded = JsonConvert.DeserializeObject<Dictionary<string, string>>(
						File.ReadAllText(_settingPath));
					if (loaded != null)
					{
						foreach (KeyValuePair<string, string> pair in loaded)
						{
							_values[pair.Key] = pair.Value;
						}
					}
				}
			}
			catch
			{
				// 配置损坏时按空字典处理，全部回落到默认值。
			}
		}

		private void Flush()
		{
			try
			{
				string dir = Path.GetDirectoryName(_settingPath);
				if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				File.WriteAllText(_settingPath, JsonConvert.SerializeObject(_values, Formatting.Indented));
			}
			catch
			{
				// 配置写入失败不影响主流程（与旧 INI 行为一致）。
			}
		}

		private string Read(string section, string key)
		{
			lock (_syncRoot)
			{
				EnsureLoaded();
				string value;
				return _values.TryGetValue(MakeKey(section, key), out value) ? value : null;
			}
		}

		private void Write(string section, string key, string value)
		{
			lock (_syncRoot)
			{
				EnsureLoaded();
				_values[MakeKey(section, key)] = value ?? string.Empty;
				Flush();
			}
		}

		public T GetSettingValue<T>(string key, T defaultValue, Func<string, T, T> convertFunc, string section, string path)
		{
			string text = Read(section, key);
			return string.IsNullOrEmpty(text) ? defaultValue : convertFunc(text, defaultValue);
		}

		public string GetSettingValue(string key, string defaultValue, string section, string path)
		{
			return GetSettingValue(key, defaultValue, (v, d) => !string.IsNullOrEmpty(v) ? v : d, section, path);
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
			return GetSettingValue(key, defaultValue, (v, d) =>
			{
				int result;
				return !int.TryParse(v, out result) ? d : result;
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
			return GetSettingValue(key, defaultValue, (v, d) =>
			{
				int result;
				return !int.TryParse(v, out result) ? d : result == 1;
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
			Write(section, key, convertFunc(value));
		}

		public void SetSettingValue(string key, string value, string section, string path)
		{
			SetSettingValue(key, value, v => v, section, path);
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
			SetSettingValue(key, value, v => v.ToString(CultureInfo.InvariantCulture), section, path);
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
			SetSettingValue(key, value, v => v ? "1" : "0", section, path);
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
