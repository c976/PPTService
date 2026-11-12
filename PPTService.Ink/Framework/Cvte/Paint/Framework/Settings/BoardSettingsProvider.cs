using System;
using System.Collections.Generic;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework.Settings
{
	public sealed class BoardSettingsProvider
	{
		[Obsolete("尽量从 Board 获取值，这个属性不保证获取到的值是属于哪个Board的设置")]
		public static BoardSettingsProvider Current { get; private set; }

		public Board Board { get; set; }

		[Obsolete("请使用 Board.GetSettings<ThresholdBoardSettings>(); 代替")]
		public ThresholdBoardSettings ThresholdBoardSettings => GetSettings<ThresholdBoardSettings>();

		private Dictionary<Type, BoardSettings> SettingDictionary { get; } = new Dictionary<Type, BoardSettings>();

		internal BoardSettingsProvider()
		{
			Current = Current ?? this;
		}

		internal T GetSettings<T>() where T : BoardSettings, new()
		{
			Type typeFromHandle = typeof(T);
			if (SettingDictionary.ContainsKey(typeFromHandle))
			{
				return (T)SettingDictionary[typeFromHandle];
			}
			T val = new T
			{
				Board = Board
			};
			SettingDictionary[typeFromHandle] = val;
			return val;
		}

		private T GetSettings<T>(Type type) where T : BoardSettings
		{
			if (SettingDictionary.ContainsKey(type))
			{
				return (T)SettingDictionary[type];
			}
			if (DebuggingProperties.IsDebug && type.GetConstructor(Type.EmptyTypes) == null)
			{
				throw new ArgumentException("提供的 BoardSettings 必须有空参数构造函数");
			}
			T val = (T)Activator.CreateInstance(type);
			val.Board = Board;
			SettingDictionary[type] = val;
			return val;
		}

		public void ConfigFrom<T>([NotNull] T setting) where T : BoardSettings, new()
		{
			if (setting == null)
			{
				throw new ArgumentNullException("设置为空，请检查设置");
			}
			T settings = GetSettings<T>();
			Clone.CloneObjectWithIL(setting, settings);
			settings.Board = Board;
		}

		private void ConfigFrom<T>([NotNull] T setting, Type settingType) where T : BoardSettings
		{
			T settings = GetSettings<T>(settingType);
			Clone.CloneObjectWithIL(setting, settings);
			settings.Board = Board;
		}

		public void UpdateBoardSettings(BoardSettingsProvider boardSettings)
		{
			foreach (KeyValuePair<Type, BoardSettings> item in boardSettings.SettingDictionary)
			{
				Type type = item.GetType();
				if (SettingDictionary.ContainsKey(type))
				{
					ConfigFrom(item.Value, type);
				}
				else
				{
					SettingDictionary.Add(type, item.Value.MemberwiseClone());
				}
			}
		}
	}
}
