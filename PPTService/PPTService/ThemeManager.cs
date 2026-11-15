using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace PPTService
{
	/// <summary>
	/// 界面主题：浅色 / 深色 / 跟随系统。
	///
	/// 通过 Application.Resources 里的一组 DynamicResource 画刷统一驱动工具栏面板、
	/// 颜色盘 / 清屏滑杆弹窗、翻页按钮底板、图标、设置窗口底板与文字等配色。
	/// 按用户定版要求：**WPF 原生按钮 / 下拉框 / 复选框等控件一律保持系统默认配色**，
	/// 不覆盖 SystemColors，也不给它们套主题画刷；板中板白板画布恒为白色。
	///
	/// 工具栏相关的半透明底板（PanelBackground / PopupBackground / PageButtonBackground）
	/// 统一使用用户设置的「工具栏透明度」（20–100%，默认 70%），只改变画刷 alpha，
	/// RGB 仍由明暗主题决定；设置窗口底板、二维码遮罩、滑杆轨道、白板画布不参与。
	///
	/// 切换主题时替换画刷对象（ResourceDictionary 会冻结放进去的 Freezable，
	/// 不能直接改已冻结画刷的 Color），并重新挂载资源字典以触发 ResourcesChanged，
	/// 让所有 {DynamicResource Theme.*}（含 DrawingImage 里的图标画刷）即时刷新。
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
			"Theme.PanelBackground",        // 工具栏面板 / 退出按钮
			"Theme.PopupBackground",        // 颜色盘 / 清屏滑杆弹窗 / 消息提示
			"Theme.SliderBackground",       // 清屏滑杆轨道
			"Theme.PrimaryText",            // 页码 / 主要文字
			"Theme.HintText",               // 滑杆提示 / 次要文字
			"Theme.BorderBrush",            // 面板描边
			"Theme.SettingsBackground",     // 设置窗口底板
			"Theme.SettingsForeground",     // 设置窗口文字
			"Theme.SettingsControl",        // 缩略图底板 / 放大镜按钮条
			"Theme.SettingsMutedText",      // 说明性文字
			"Theme.MaskBackground",         // 全屏窗口的半透明遮罩
			"Theme.PageButtonBackground",   // 翻页按钮底板（跟随工具栏透明度）
			"Theme.IconBrush"               // 工具栏矢量图标：浅色深灰 / 深色浅灰
		};

		private static ResourceDictionary _brushes;
		private static string _theme = ThemeLight;
		private static int _toolbarOpacity = UserConfig.DefaultToolbarOpacity;
		private static bool _systemEventsHooked;

		public static string Normalize(string theme)
		{
			if (theme == ThemeDark || theme == ThemeSystem)
			{
				return theme;
			}
			return ThemeLight;
		}

		/// <summary>按当前用户配置应用主题与工具栏透明度（启动时、设置改动时调用）。</summary>
		public static void Apply(UserConfig cfg)
		{
			Apply(cfg == null ? ThemeLight : cfg.Theme,
				cfg == null ? UserConfig.DefaultToolbarOpacity : cfg.ToolbarOpacity);
		}

		public static void Apply(string theme)
		{
			Apply(theme, UserConfig.DefaultToolbarOpacity);
		}

		public static void Apply(string theme, int toolbarOpacity)
		{
			_theme = Normalize(theme);
			_toolbarOpacity = UserConfig.NormalizeToolbarOpacity(toolbarOpacity);
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
			EnsureDictionary();
			bool dark = IsDark(_theme);
			// 工具栏相关底板统一使用同一个 alpha；RGB 仍随明暗主题变化。
			byte panelAlpha = (byte)Math.Round(_toolbarOpacity * 255.0 / 100.0);
			// 图标里以 <SolidColorBrush Color="#FF666666"> 形式写的深灰（带 Transform，
			// 不便改成 Brush 属性）用 Color 级资源跟随主题。
			app.Resources["Theme.IconColor"] = dark
				? Color.FromRgb(0xD8, 0xD8, 0xD8)
				: Color.FromRgb(0x66, 0x66, 0x66);
			if (dark)
			{
				Set("Theme.PanelBackground", panelAlpha, 0x2B, 0x2B, 0x31);
				Set("Theme.PopupBackground", panelAlpha, 0x2B, 0x2B, 0x31);
				Set("Theme.SliderBackground", 0xFF, 0x4A, 0x4A, 0x52);
				Set("Theme.PrimaryText", 0xFF, 0xEC, 0xEC, 0xEC);
				Set("Theme.HintText", 0xFF, 0xB4, 0xB4, 0xB4);
				Set("Theme.BorderBrush", 0x80, 0xB0, 0xB0, 0xB0);
				Set("Theme.SettingsBackground", 0xFF, 0x23, 0x23, 0x28);
				Set("Theme.SettingsForeground", 0xFF, 0xF0, 0xF0, 0xF0);
				Set("Theme.SettingsControl", 0xFF, 0x35, 0x35, 0x3C);
				Set("Theme.SettingsMutedText", 0xFF, 0xA8, 0xA8, 0xA8);
				Set("Theme.MaskBackground", 0xB3, 0x00, 0x00, 0x00);
				Set("Theme.PageButtonBackground", panelAlpha, 0x2B, 0x2B, 0x31);
				Set("Theme.IconBrush", 0xFF, 0xD8, 0xD8, 0xD8);
			}
			else
			{
				Set("Theme.PanelBackground", panelAlpha, 0xFF, 0xFF, 0xFF);
				Set("Theme.PopupBackground", panelAlpha, 0xFF, 0xFF, 0xFF);
				Set("Theme.SliderBackground", 0xFF, 0xEB, 0xEB, 0xED);
				Set("Theme.PrimaryText", 0xFF, 0x66, 0x66, 0x66);
				Set("Theme.HintText", 0xFF, 0x98, 0x98, 0x98);
				Set("Theme.BorderBrush", 0x80, 0x66, 0x66, 0x66);
				Set("Theme.SettingsBackground", 0xFF, 0xFF, 0xFF, 0xFF);
				Set("Theme.SettingsForeground", 0xFF, 0x1A, 0x1A, 0x1A);
				Set("Theme.SettingsControl", 0xFF, 0xF7, 0xF7, 0xF7);
				Set("Theme.SettingsMutedText", 0xFF, 0x80, 0x80, 0x80);
				Set("Theme.MaskBackground", 0x66, 0x00, 0x00, 0x00);
				Set("Theme.PageButtonBackground", panelAlpha, 0xFF, 0xFF, 0xFF);
				Set("Theme.IconBrush", 0xFF, 0x66, 0x66, 0x66);
			}
			// 只替换字典里的画刷对象不会让「已解析」的 DynamicResource 重新查找。
			// 移出再挂回同一个字典可触发 ResourcesChanged，让所有引用（含设置窗口
			// 打开期间）立刻取到新画刷，无需关掉窗口重开。
			app.Resources.MergedDictionaries.Remove(_brushes);
			app.Resources.MergedDictionaries.Add(_brushes);
		}

		private static void EnsureDictionary()
		{
			if (_brushes != null)
			{
				return;
			}
			_brushes = new ResourceDictionary();
			foreach (string key in BrushKeys)
			{
				// 占位用不透明白色：万一后续配色刷新失败，界面仍是原版浅色外观，
				// 不会因为 DynamicResource 取到透明画刷而「看不见」。
				_brushes[key] = new SolidColorBrush(Colors.White);
			}
		}

		private static void Set(string key, byte a, byte r, byte g, byte b)
		{
			// ResourceDictionary 会自动冻结（Freeze）放进去的 Freezable，
			// 已冻结的画刷不能改 Color 属性（抛 InvalidOperationException）。
			// 因此每次刷新都替换整支画刷对象，再由 Refresh() 重新挂载字典触发刷新。
			_brushes[key] = new SolidColorBrush(Color.FromArgb(a, r, g, b));
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
