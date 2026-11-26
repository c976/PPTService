using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace PPTServiceUltra
{
	/// <summary>
	/// 界面主题：浅色 / 深色 / 跟随系统。
	///
	/// 【工具栏透明度定版规则】透明度只作用于「背景」画刷的 alpha：
	///   - 背景字典（PanelBackground / PopupBackground / PageButtonBackground）
	///     随滑条变化，RGB 由明暗主题决定；
	///   - 内容字典（图标 / 文字 / 设置窗口 / 遮罩 / 滑杆轨道等）画刷 alpha
	///     恒为 0xFF（100% 不透明），并且拖动透明度滑条时**完全不重建、不重挂**，
	///     图标与文字的 DynamicResource 引用保持不变。
	///   - 绝不使用 UIElement.Opacity 实现背景透明度（那是「翻页按钮自动淡化」
	///     用的整块虚化，会把图标和文字一起变淡，与这里的需求不同）。
	///
	/// 画刷放进 ResourceDictionary 后会被冻结，不能直接改已冻结画刷的 Color，
	/// 因此每次刷新都替换整支画刷对象，并重挂对应字典触发 ResourcesChanged，
	/// 让 {DynamicResource Theme.*}（含 DrawingImage 里的图标画刷）即时刷新。
	///
	/// 「跟随系统」读取 HKCU\...\Themes\Personalize\AppsUseLightTheme，
	/// 并订阅 SystemEvents.UserPreferenceChanged，在系统主题变化后自动重新应用。
	/// </summary>
	public static class ThemeManager
	{
		public const string ThemeLight = "Light";
		public const string ThemeDark = "Dark";
		public const string ThemeSystem = "System";

		// 背景低于此不透明度时，幻灯片（多为白底）会大量透出，深色主题的浅灰图标
		// 会失去对比度。仅跨越此阈值时切换工具栏内容的 RGB，alpha 仍恒 0xFF。
		private const int LowOpacityThreshold = 55;

		// 唯一受「工具栏透明度」影响的三个背景画刷。
		private static readonly string[] BackgroundKeys =
		{
			"Theme.PanelBackground",        // 工具栏面板 / 退出按钮
			"Theme.PopupBackground",        // 颜色盘 / 清屏滑杆弹窗 / 消息提示
			"Theme.PageButtonBackground"    // 翻页按钮底板
		};

		// 内容画刷：alpha 恒 0xFF，不随透明度滑条变化。
		private static readonly string[] ContentKeys =
		{
			"Theme.SliderBackground",       // 清屏滑杆轨道
			"Theme.PrimaryText",            // 页码 / 主要文字
			"Theme.HintText",               // 滑杆提示 / 次要文字
			"Theme.BorderBrush",            // 面板描边
			"Theme.SettingsBackground",     // 设置窗口底板
			"Theme.SettingsForeground",     // 设置窗口文字
			"Theme.SettingsControl",        // 缩略图底板 / 放大镜按钮条
			"Theme.SettingsMutedText",      // 说明性文字
			"Theme.ThumbnailText",          // 缩略图页码（压在不透明设置面板上，只随明暗主题）
			"Theme.MaskBackground",         // 全屏窗口的半透明遮罩
			"Theme.IconBrush",              // 工具栏矢量图标：浅色深灰 / 深色浅灰
			"Theme.EraserThumbFace",        // 橡皮清屏滑杆拇指圆盘底色
			"Theme.EraserThumbRing",        // 橡皮清屏滑杆拇指外圈描边
			"Theme.EraserThumbBar"          // 橡皮清屏滑杆拇指三条竖线
		};

		private static ResourceDictionary _backgroundBrushes;
		private static ResourceDictionary _contentBrushes;
		private static string _theme = ThemeLight;
		private static int _toolbarOpacity = UserConfig.DefaultToolbarOpacity;
		private static bool _contentReady;
		private static bool _lowContrastMode;
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
			string normalized = Normalize(theme);
			int opacity = UserConfig.NormalizeToolbarOpacity(toolbarOpacity);
			bool lowContrast = opacity < LowOpacityThreshold;
			// 只在明暗主题 / 高对比模式真的变化（或首次）时重建内容字典；
			// 同一区间内拖动透明度滑条走 backgroundOnly，图标 / 文字画刷连重查都不会发生。
			bool themeChanged = !_contentReady || normalized != _theme || lowContrast != _lowContrastMode;
			_theme = normalized;
			_toolbarOpacity = opacity;
			_lowContrastMode = lowContrast;
			HookSystemEvents();
			Refresh(!themeChanged);
			_contentReady = true;
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
			app.Dispatcher.BeginInvoke(new Action(() => Refresh(false)));
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

		/// <param name="backgroundOnly">
		/// true = 只重建背景字典（拖动透明度滑条）；内容画刷保持原对象不动。
		/// false = 明暗主题变化，内容画刷也要按新配色重建并重挂。
		/// </param>
		private static void Refresh(bool backgroundOnly)
		{
			Application app = Application.Current;
			if (app == null)
			{
				return;
			}
			EnsureDictionaries(app);
			bool dark = IsDark(_theme);

			if (!backgroundOnly)
			{
				// 工具栏内容（图标 / 页码 / 提示文字）：alpha 恒 0xFF，只按「实际可见背景」
				// 选高对比 RGB。深色主题下背景很透时幻灯片白底会透出来，浅灰图标会消失，
				// 此时切回深灰；默认 70% 不透明时仍是用户定版的「深色浅灰图标」。
				bool lightContent = dark && !_lowContrastMode;
				_contentBrushes["Theme.IconColor"] = lightContent
					? Color.FromRgb(0xD8, 0xD8, 0xD8)
					: Color.FromRgb(0x66, 0x66, 0x66);
				if (lightContent)
				{
					SetContent("Theme.PrimaryText", 0xEC, 0xEC, 0xEC);
					SetContent("Theme.HintText", 0xB4, 0xB4, 0xB4);
					SetContent("Theme.IconBrush", 0xD8, 0xD8, 0xD8);
				}
				else
				{
					SetContent("Theme.PrimaryText", 0x66, 0x66, 0x66);
					SetContent("Theme.HintText", 0x98, 0x98, 0x98);
					SetContent("Theme.IconBrush", 0x66, 0x66, 0x66);
				}

				// 以下只随明暗主题，绝不受背景透明度影响：设置窗口、缩略图、遮罩、滑杆轨道。
				// 缩略图页码用独立的 ThumbnailText（它压在不透明的设置面板底板上，
				// 不能跟翻页区页码一起按背景透明度换色）。
				if (dark)
				{
					SetContent("Theme.SliderBackground", 0x4A, 0x4A, 0x52);
					Set("Theme.BorderBrush", 0x80, 0xB0, 0xB0, 0xB0);
					SetContent("Theme.SettingsBackground", 0x23, 0x23, 0x28);
					SetContent("Theme.SettingsForeground", 0xF0, 0xF0, 0xF0);
					SetContent("Theme.SettingsControl", 0x35, 0x35, 0x3C);
					SetContent("Theme.SettingsMutedText", 0xA8, 0xA8, 0xA8);
					SetContent("Theme.ThumbnailText", 0xEC, 0xEC, 0xEC);
					Set("Theme.MaskBackground", 0xB3, 0x00, 0x00, 0x00);
				}
				else
				{
					SetContent("Theme.SliderBackground", 0xEB, 0xEB, 0xED);
					Set("Theme.BorderBrush", 0x80, 0x66, 0x66, 0x66);
					SetContent("Theme.SettingsBackground", 0xFF, 0xFF, 0xFF);
					SetContent("Theme.SettingsForeground", 0x1A, 0x1A, 0x1A);
					SetContent("Theme.SettingsControl", 0xF7, 0xF7, 0xF7);
					SetContent("Theme.SettingsMutedText", 0x80, 0x80, 0x80);
					SetContent("Theme.ThumbnailText", 0x66, 0x66, 0x66);
					Set("Theme.MaskBackground", 0x66, 0x00, 0x00, 0x00);
				}
				// 橡皮拇指：浅色按参考图（白圆盘 + 浅灰细竖线）；深色改为深灰圆盘 +
				// 稍浅竖线（与底色明显不同即可）。低透明度高对比时退回白底深线，
				// 避免幻灯片白底透出后浅色竖线消失。
				if (lightContent)
				{
					SetContent("Theme.EraserThumbFace", 0x35, 0x35, 0x3C);
					SetContent("Theme.EraserThumbRing", 0x50, 0x50, 0x58);
					SetContent("Theme.EraserThumbBar", 0xB4, 0xB4, 0xBC);
				}
				else
				{
					SetContent("Theme.EraserThumbFace", 0xFF, 0xFF, 0xFF);
					SetContent("Theme.EraserThumbRing", 0xEB, 0xEB, 0xED);
					if (dark)
					{
						SetContent("Theme.EraserThumbBar", 0x66, 0x66, 0x66);
					}
					else
					{
						SetContent("Theme.EraserThumbBar", 0xD3, 0xD3, 0xD3);
					}
				}

				// 替换画刷对象后重挂字典，触发 ResourcesChanged（仅主题 / 高对比模式切换时）。
				app.Resources.MergedDictionaries.Remove(_contentBrushes);
				app.Resources.MergedDictionaries.Add(_contentBrushes);
			}

			// 背景画刷：唯一读取工具栏透明度的地方，每次刷新都按当前 alpha 重建。
			if (dark)
			{
				SetBackground("Theme.PanelBackground", 0x2B, 0x2B, 0x31);
				SetBackground("Theme.PopupBackground", 0x2B, 0x2B, 0x31);
				SetBackground("Theme.PageButtonBackground", 0x2B, 0x2B, 0x31);
			}
			else
			{
				SetBackground("Theme.PanelBackground", 0xFF, 0xFF, 0xFF);
				SetBackground("Theme.PopupBackground", 0xFF, 0xFF, 0xFF);
				SetBackground("Theme.PageButtonBackground", 0xFF, 0xFF, 0xFF);
			}
			app.Resources.MergedDictionaries.Remove(_backgroundBrushes);
			app.Resources.MergedDictionaries.Add(_backgroundBrushes);
		}

		private static void EnsureDictionaries(Application app)
		{
			if (_backgroundBrushes == null)
			{
				_backgroundBrushes = new ResourceDictionary();
				foreach (string key in BackgroundKeys)
				{
					// 占位用不透明白色：万一后续配色刷新失败，界面仍是原版浅色外观。
					_backgroundBrushes[key] = new SolidColorBrush(Colors.White);
				}
				app.Resources.MergedDictionaries.Add(_backgroundBrushes);
			}
			if (_contentBrushes == null)
			{
				_contentBrushes = new ResourceDictionary();
				foreach (string key in ContentKeys)
				{
					_contentBrushes[key] = new SolidColorBrush(Colors.White);
				}
				_contentBrushes["Theme.IconColor"] = Color.FromRgb(0x66, 0x66, 0x66);
				app.Resources.MergedDictionaries.Add(_contentBrushes);
			}
		}

		/// <summary>工具栏背景不透明度换算成画刷 alpha（20–100% → 51–255）。</summary>
		private static byte BackgroundAlpha()
		{
			return (byte)Math.Round(UserConfig.NormalizeToolbarOpacity(_toolbarOpacity) * 255.0 / 100.0);
		}

		/// <summary>背景画刷：alpha 只由工具栏透明度决定，不影响文字与图标。</summary>
		private static void SetBackground(string key, byte r, byte g, byte b)
		{
			Set(key, BackgroundAlpha(), r, g, b);
		}

		/// <summary>文字 / 图标画刷：alpha 恒为 0xFF（100% 不透明），永远不随背景透明度变化。</summary>
		private static void SetContent(string key, byte r, byte g, byte b)
		{
			Set(key, 0xFF, r, g, b);
		}

		private static void Set(string key, byte a, byte r, byte g, byte b)
		{
			// ResourceDictionary 会自动冻结（Freeze）放进去的 Freezable，
			// 已冻结的画刷不能改 Color 属性（抛 InvalidOperationException）。
			// 因此每次刷新都替换整支画刷对象，再由 Refresh() 重挂对应字典触发刷新。
			ResourceDictionary dict = Array.IndexOf(BackgroundKeys, key) >= 0
				? _backgroundBrushes
				: _contentBrushes;
			dict[key] = new SolidColorBrush(Color.FromArgb(a, r, g, b));
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
