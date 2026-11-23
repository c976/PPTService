using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Configurations
{
	[StructLayout(LayoutKind.Auto)]
	
	
	
	public struct ConfigurationString : IEquatable<ConfigurationString>
	{
		
		private readonly string _value;

		private ConfigurationString(string value)
		{
			_value = value;
		}

		
		public static implicit operator ConfigurationString?(string value)
		{
			return Convert(value);
		}

		
		private static ConfigurationString? Convert(string value)
		{
			if (value != null && !string.IsNullOrEmpty(value))
			{
				return new ConfigurationString(value);
			}
			return null;
		}

		public static implicit operator string(ConfigurationString? configurationValue)
		{
			return configurationValue?.ToString() ?? string.Empty;
		}

		public override string ToString()
		{
			if (_value != null && !string.IsNullOrEmpty(_value))
			{
				return _value;
			}
			return string.Empty;
		}

		
		public override bool Equals(object other)
		{
			if (other is ConfigurationString)
			{
				ConfigurationString other2 = (ConfigurationString)other;
				return Equals(other2);
			}
			string text = other as string;
			if (text != null)
			{
				return Equals(text);
			}
			return false;
		}

		public bool Equals(ConfigurationString other)
		{
			return _value == other._value;
		}

		public bool Equals(string other)
		{
			return _value == other;
		}

		public override int GetHashCode()
		{
			if (!string.IsNullOrEmpty(_value))
			{
				return StringComparer.Ordinal.GetHashCode(_value);
			}
			return 0;
		}

		public static bool operator ==(ConfigurationString left, ConfigurationString right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ConfigurationString left, ConfigurationString right)
		{
			return !(left == right);
		}
	}
}
