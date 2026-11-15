using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Configurations.Core
{
	
	
	
	internal struct ConfigurationValueEntry
	{
		private readonly DateTimeOffset _updatedTime;

		public string Value { get; }

		public ConfigurationValueEntry(string value)
		{
			Value = value;
			_updatedTime = DateTimeOffset.Now;
		}

		public static ConfigurationValueEntry GetLatest(ConfigurationValueEntry value1, ConfigurationValueEntry value2)
		{
			if (!(value1._updatedTime > value2._updatedTime))
			{
				return value2;
			}
			return value1;
		}

		[Obsolete("不含时间戳的字符串无法当作配置项比较新旧。", true)]
		public static ConfigurationValueEntry GetLatest(string value1, string value2)
		{
			throw new NotSupportedException("不含时间戳的字符串无法当作配置项比较新旧。");
		}

		[Obsolete("不含时间戳的字符串无法当作配置项比较新旧。", true)]
		public static ConfigurationValueEntry GetLatest(string value1, ConfigurationValueEntry value2)
		{
			throw new NotSupportedException("不含时间戳的字符串无法当作配置项比较新旧。");
		}

		[Obsolete("不含时间戳的字符串无法当作配置项比较新旧。", true)]
		public static ConfigurationValueEntry GetLatest(ConfigurationValueEntry value1, string value2)
		{
			throw new NotSupportedException("不含时间戳的字符串无法当作配置项比较新旧。");
		}

		public static implicit operator ConfigurationValueEntry(string value)
		{
			return new ConfigurationValueEntry(value);
		}

		public static implicit operator string(ConfigurationValueEntry entry)
		{
			return entry.Value;
		}
	}
}
