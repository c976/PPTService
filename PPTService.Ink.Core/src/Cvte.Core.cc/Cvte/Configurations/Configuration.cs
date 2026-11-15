using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Configurations.Core;

namespace PPTService.Ink.Core.Configurations
{
	
	
	public abstract class Configuration
	{
		
		private readonly string _section;

		internal IConfigurationRepo Repo { get; set; }

		protected Configuration()
		{
			string classNameWithoutSuffix = this.GetClassNameWithoutSuffix();
			_section = classNameWithoutSuffix + ".";
		}

		protected Configuration(string sectionName)
		{
			_section = (string.IsNullOrEmpty(sectionName) ? "" : (sectionName + "."));
		}

		protected bool? GetBoolean([CallerMemberName] string key = null)
		{
			string value = GetValue(key);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return bool.Parse(value);
			}
			return null;
		}

		protected decimal? GetDecimal([CallerMemberName] string key = null)
		{
			string value = GetValue(key);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return decimal.Parse(value, CultureInfo.InvariantCulture);
			}
			return null;
		}

		protected double? GetDouble([CallerMemberName] string key = null)
		{
			string value = GetValue(key);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return double.Parse(value, CultureInfo.InvariantCulture);
			}
			return null;
		}

		protected float? GetSingle([CallerMemberName] string key = null)
		{
			string value = GetValue(key);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return float.Parse(value, CultureInfo.InvariantCulture);
			}
			return null;
		}

		protected int? GetInt32([CallerMemberName] string key = null)
		{
			string value = GetValue(key);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return int.Parse(value, CultureInfo.InvariantCulture);
			}
			return null;
		}

		protected long? GetInt64([CallerMemberName] string key = null)
		{
			string value = GetValue(key);
			if (!string.IsNullOrWhiteSpace(value))
			{
				return long.Parse(value, CultureInfo.InvariantCulture);
			}
			return null;
		}

		protected ConfigurationString? GetString([CallerMemberName] string key = null)
		{
			return GetValue(key);
		}

		internal string GetValue([CallerMemberName] string key = null)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (Repo == null)
			{
				throw new InvalidOperationException("必须通过 IAppConfigurator.Of 使用配置。");
			}
			return Repo.GetValue(_section + key);
		}

		protected void SetValue(bool? value, [CallerMemberName] string key = null)
		{
			SetValue(value?.ToString(CultureInfo.InvariantCulture) ?? "", key);
		}

		protected void SetValue(decimal? value, [CallerMemberName] string key = null)
		{
			SetValue(value?.ToString(CultureInfo.InvariantCulture) ?? "", key);
		}

		protected void SetValue(double? value, [CallerMemberName] string key = null)
		{
			SetValue(value?.ToString(CultureInfo.InvariantCulture) ?? "", key);
		}

		protected void SetValue(float? value, [CallerMemberName] string key = null)
		{
			SetValue(value?.ToString(CultureInfo.InvariantCulture) ?? "", key);
		}

		protected void SetValue(int? value, [CallerMemberName] string key = null)
		{
			SetValue(value?.ToString(CultureInfo.InvariantCulture) ?? "", key);
		}

		protected void SetValue(long? value, [CallerMemberName] string key = null)
		{
			SetValue(value?.ToString(CultureInfo.InvariantCulture) ?? "", key);
		}

		protected internal void SetValue(ConfigurationString? value, [CallerMemberName] string key = null)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (Repo == null)
			{
				throw new InvalidOperationException("必须通过 IAppConfigurator.Of 使用配置。");
			}
			Repo.SetValue(_section + key, value?.ToString());
		}

		protected void ClearValues()
		{
			if (Repo == null)
			{
				throw new InvalidOperationException("必须通过 IAppConfigurator.Of 使用配置。");
			}
			Repo.ClearValues( (string key) => key.StartsWith(_section, StringComparison.InvariantCulture));
		}
	}
}
