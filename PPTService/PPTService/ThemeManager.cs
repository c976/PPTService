using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace PPTService
{
	/// <summary>
	/// 界面主题：浅色 / 深色 / 跟随系统。
	///
	/// 通过 Application.Resources 里的一组 DynamicResource 画刷统一驱动工具栏、翻页按钮、
	/// 颜色盘 / 清屏滑杆弹窗、设置窗口、二维码窗口、消息提示等界面配色。
	/// 切换主题时直接改画刷对象的 Color，所有 {DynamicResource Theme.*} 引用即时刷新，
	/// 无需重建窗口。板中板的白板画布（InkCanvasBorder）是书写区域，不参与主题，
	/// 始终保持白色以保证彩色笔迹可读。
	///
	/// 「跟随系统」读取 HKCU\...\Themes\Personalize\AppsUseLightTheme，
	/// 并订阅 SystemEvents.UserPreferenceChanged，在系统主题变化后自动重新应用。
	/// </summary>
	public static class ThemeManager
	{
		public const string ThemeLight = "Light";
		public const string ThemeDark = "Dark";
		public const string ThemeSystem = "System";

		// 自定义画刷键：XAML 里统一用 {DynamicResource Theme.Xxx} 引用。
		private static readonly string[] BrushKeys =
		{
			"Theme.PanelBackground",        // 工具栏面板 / 退出按钮 / 翻页按钮底板
			"Theme.PopupBackground",        // 颜色盘 / 清屏滑杆弹窗 / 消息提示
			"Theme.SliderBackground",       // 清屏滑杆轨道
			"Theme.PrimaryText",            // 页码 / 主要文字
			"Theme.HintText",               // 滑杆提示 / 次要文字
			"Theme.BorderBrush",            // 面板描边
			"Theme.SettingsBackground",     // 设置窗口底板
			"Theme.SettingsForeground",     // 设置窗口文字
			"Theme.SettingsControl",        // 按钮 / 下拉框 / 列表项 / 缩略图底板
			"Theme.SettingsMutedText",      // 说明性文字
			"Theme.MaskBackground",         // 全屏窗口的半透明遮罩
			"Theme.PageButtonBackground"    // 翻页按钮（明暗主题 + 半透明开关共同决定）
		};

		// WPF 原生控件（Button / ComboBox / CheckBox / GroupBox / ListBox 等）
		// 的默认模板取自 SystemColors，深色主题下需覆盖这些键才能一起变深。
		private static readonly object[] SystemColorKeys =
		{
			SystemColors.WindowBrushKey,
			SystemColors.ControlBrushKey,
			SystemColors.ControlTextBrushKey,
			SystemColors.WindowTextBrushKey,
			SystemColors.HighlightBrushKey,
			SystemColors.HighlightTextBrushKey,
			SystemColors.ControlLightBrushKey,
			SystemColors.ControlDarkBrushKey
		};

		private static ResourceDictionary _brushes;
		private static ResourceDictionary _systemColors;
		private static string _theme = ThemeLight;
		private static bool _translucentPageButtons = true;
		private static bool _systemEventsHooked;

		public static string Normalize(string theme)
		{
			if (theme == ThemeDark || theme == ThemeSystem)
			{
				return theme;
			}
			return ThemeLight;
		}

		/// <summary>按当前用户配置应用主题（启动时与设置「应用 / 确定」时调用）。</summary>
		public static void Apply(UserConfig cfg)
		{
			Apply(cfg == null ? ThemeLight : cfg.Theme,
				cfg == null || cfg.PageTurningTranslucent);
		}

		public static void Apply(string theme, bool translucentPageButtons)
		{
			_theme = Normalize(theme);
			_translucentPageButtons = translucentPageButtons;
			HookSystemEvents();
			Refresh();
		}

		/// <summary>系统主题变化时的即时刷新（仅「跟随系统」模式生效）。</summary>
		private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
		{
			if (_theme != ThemeSystem)
			{
				return;
			}
			Application app = Application.Current;
			if (app == null)
			{
				return;
			}
			app.Dispatcher.BeginInvoke(new Action(Refresh));
		}

		private static void HookSystemEvents()
		{
			if (_systemEventsHooked)
			{
				return;
			}
			_systemEventsHooked = true;
			try
			{
				SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
			}
			catch
			{
				// 极端环境（无消息循环 / 权限受限）下拿不到系统事件：仍可在启动与设置应用时跟随。
			}
		}

		private static void Refresh()
		{
			Application app = Application.Current;
			if (app == null)
			{
				return;
			}
			EnsureDictionary(app);
			bool dark = IsDark(_theme);
			byte pageAlpha = (byte)(_translucentPageButtons ? 0xA0 : 0xFF);
			if (dark)
			{
				Set("Theme.PanelBackground", 0xCC, 0x2B, 0x2B, 0x31);
				Set("Theme.PopupBackground", 0xF2, 0x2B, 0x2B, 0x31);
				Set("Theme.SliderBackground", 0xFF, 0x4A, 0x4A, 0x52);
				Set("Theme.PrimaryText", 0xFF, 0xEC, 0xEC, 0xEC);
				Set("Theme.HintText", 0xFF, 0xB4, 0xB4, 0xB4);
				Set("Theme.BorderBrush", 0x80, 0xB0, 0xB0, 0xB0);
				Set("Theme.SettingsBackground", 0xFF, 0x23, 0x23, 0x28);
				Set("Theme.SettingsForeground", 0xFF, 0xF0, 0xF0, 0xF0);
				Set("Theme.SettingsControl", 0xFF, 0x35, 0x35, 0x3C);
				Set("Theme.SettingsMutedText", 0xFF, 0xA8, 0xA8, 0xA8);
				Set("Theme.MaskBackground", 0xB3, 0x00, 0x00, 0x00);
				Set("Theme.PageButtonBackground", pageAlpha, 0x2B, 0x2B, 0x31);

				OverrideSystemColors(true);
			}
			else
			{
				Set("Theme.PanelBackground", 0xA0, 0xFF, 0xFF, 0xFF);
				Set("Theme.PopupBackground", 0xE6, 0xFF, 0xFF, 0xFF);
				Set("Theme.SliderBackground", 0xFF, 0xEB, 0xEB, 0xED);
				Set("Theme.PrimaryText", 0xFF, 0x66, 0x66, 0x66);
				Set("Theme.HintText", 0xFF, 0x98, 0x98, 0x98);
				Set("Theme.BorderBrush", 0x80, 0x66, 0x66, 0x66);
				Set("Theme.SettingsBackground", 0xFF, 0xFF, 0xFF, 0xFF);
				Set("Theme.SettingsForeground", 0xFF, 0x1A, 0x1A, 0x1A);
				Set("Theme.SettingsControl", 0xFF, 0xF7, 0xF7, 0xF7);
				Set("Theme.SettingsMutedText", 0xFF, 0x80, 0x80, 0x80);
				Set("Theme.MaskBackground", 0x66, 0x00, 0x00, 0x00);
				Set("Theme.PageButtonBackground", pageAlpha, 0xFF, 0xFF, 0xFF);

				OverrideSystemColors(false);
			}
		}

		/// <summary>浅色时移除覆盖，恢复系统默认画刷；深色时压入深色画刷。</summary>
		private static void OverrideSystemColors(bool dark)
		{
			foreach (object key in SystemColorKeys)
			{
				if (!dark)
				{
					((IDictionary)_systemColors).Remove(key);
					continue;
				}
				SolidColorBrush brush;
				if (key.Equals(SystemColors.HighlightBrushKey))
				{
					brush = new SolidColorBrush(Color.FromRgb(0x0F, 0x80, 0xFB));
				}
				else if (key.Equals(SystemColors.HighlightTextBrushKey))
				{
					brush = new SolidColorBrush(Colors.White);
				}
				else if (key.Equals(SystemColors.ControlLightBrushKey))
				{
					brush = new SolidColorBrush(Color.FromRgb(0x4A, 0x4A, 0x52));
				}
				else if (key.Equals(SystemColors.ControlDarkBrushKey))
				{
					brush = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x22));
				}
				else if (key.Equals(SystemColors.ControlTextBrushKey) || key.Equals(SystemColors.WindowTextBrushKey))
				{
					brush = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
				}
				else
				{
					brush = new SolidColorBrush(Color.FromRgb(0x35, 0x35, 0x3C));
				}
				_systemColors[key] = brush;
			}
		}

		private static void EnsureDictionary(Application app)
		{
			if (_brushes != null)
			{
				return;
			}
			_brushes = new ResourceDictionary();
			foreach (string key in BrushKeys)
			{
				_brushes[key] = new SolidColorBrush(Colors.Transparent);
			}
			app.Resources.MergedDictionaries.Add(_brushes);
			// SystemColors 键必须放在应用资源根字典：原生控件模板用
			// {StaticResource {x:Static SystemColors.XxxKey}}，只查根级资源更可靠。
			_systemColors = app.Resources;
		}

		private static void Set(string key, byte a, byte r, byte g, byte b)
		{
			((SolidColorBrush)_brushes[key]).Color = Color.FromArgb(a, r, g, b);
		}

		/// <summary>当前生效的明暗（跟随系统时读注册表，读取失败按浅色处理）。</summary>
		private static bool IsDark(string theme)
		{
			if (theme == ThemeDark)
			{
				return true;
			}
			if (theme != ThemeSystem)
			{
				return false;
			}
			try
			{
				using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
					@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
				{
					object value = key == null ? null : key.GetValue("AppsUseLightTheme");
					if (value is int)
					{
						return (int)value == 0;
					}
					if (value != null && value.ToString() == "0")
					{
						return true;
					}
				}
			}
			catch
			{
				// 老系统 / 无权限：按浅色。
			}
			return false;
		}
	}
}
