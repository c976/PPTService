using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Configurations.Converters
{
	
	
	public static class ConfigurationExtensions
	{
		
		public static T? GetValue<T>( this Configuration @this, [CallerMemberName] string key = null) where T : struct
		{
			if (@this == null)
			{
				throw new ArgumentNullException("this");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			Type typeFromHandle = typeof(T);
			string value = @this.GetValue(key);
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}
			object obj = TypeDescriptor.GetConverter(typeFromHandle).ConvertFromInvariantString(value);
			if (obj == null)
			{
				throw new NotSupportedException("出现了不可能的情况，从字符串 " + value + " 转换为值类型 " + typeof(T).FullName + " 时，转换后的值为 null。");
			}
			return (T)obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		public static void SetValue<T>( this Configuration @this, T value, [CallerMemberName] string key = null) where T : struct
		{
			@this.SetValue((T?)value, key);
		}

		
		public static void SetValue<T>( this Configuration @this, T? value, [CallerMemberName] string key = null) where T : struct
		{
			if (@this == null)
			{
				throw new ArgumentNullException("this");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			Type typeFromHandle = typeof(T);
			if (!value.HasValue)
			{
				@this.SetValue(string.Empty, key);
				return;
			}
			string text = TypeDescriptor.GetConverter(typeFromHandle).ConvertToInvariantString(value.Value);
			if (text == null)
			{
				throw new NotSupportedException($"无法从类型 {typeFromHandle.FullName} 将值 {value.Value} 转换成字符串。");
			}
			@this.SetValue(text, key);
		}

		[Obsolete("请改用 DateTimeOffset 类型，因为 DateTime 在存储和传输过程中会丢失时区信息导致值在读写后发生变化。")]
		public static void SetValue(this Configuration @this, DateTime value, [CallerMemberName] string key = null)
		{
			SetValueCore(@this, key, value.ToString("O", CultureInfo.InvariantCulture));
		}

		public static void SetValue(this Configuration @this, DateTimeOffset value, [CallerMemberName] string key = null)
		{
			SetValueCore(@this, key, value.ToString("O", CultureInfo.InvariantCulture));
		}

		public static void SetValue(this Configuration @this, DateTimeOffset? value, [CallerMemberName] string key = null)
		{
			SetValueCore(@this, key, value?.ToString("O", CultureInfo.InvariantCulture));
		}

		public static void SetValue(this Configuration @this, TimeSpan value, [CallerMemberName] string key = null)
		{
			SetValueCore(@this, key, value.ToString("O", CultureInfo.InvariantCulture));
		}

		public static void SetValue(this Configuration @this, TimeSpan? value, [CallerMemberName] string key = null)
		{
			SetValueCore(@this, key, value?.ToString("O", CultureInfo.InvariantCulture));
		}

		
		private static void SetValueCore( Configuration configs, string key, string value)
		{
			if (configs == null)
			{
				throw new ArgumentNullException("configs");
			}
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			configs.SetValue(value, key);
		}
	}
}
