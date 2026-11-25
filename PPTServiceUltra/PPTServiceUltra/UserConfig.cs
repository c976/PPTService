using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PPTServiceUltra.Collection;

namespace PPTServiceUltra
{
    /// <summary>
    /// 单个工具栏按钮的配置项。
    /// </summary>
    public class ToolbarButtonSetting
    {
        public string Id { get; set; }
        public bool Visible { get; set; }
    }

    /// <summary>
    /// 用户配置根对象，序列化到 %APPDATA%\PPTServiceUltra\UserConfig.json。
    /// </summary>
    public class UserConfig
    {
        /// <summary>工具栏背景不透明度允许的最小值（百分比）。</summary>
        public const int MinToolbarOpacity = 20;

        /// <summary>工具栏背景不透明度允许的最大值（百分比）。</summary>
        public const int MaxToolbarOpacity = 100;

        /// <summary>工具栏背景不透明度默认值（百分比）。</summary>
        public const int DefaultToolbarOpacity = 70;

        /// <summary>
        /// 工具栏相关半透明底板（工具栏面板 / 退出按钮 / 翻页按钮 / 颜色盘与清屏滑杆弹窗等）
        /// 统一使用的不透明度，百分比 20–100，默认 70。由 ThemeManager 换算成画刷 alpha。
        /// </summary>
        public int ToolbarOpacity { get; set; }

        /// <summary>
        /// 工具栏中央功能按钮列表，顺序即显示顺序；Visible 控制显隐。
        /// 仅包含这 5 个功能按钮：Select / Pen / Rubber / Magnifier / InnerBoard（板中板）。
        /// </summary>
        public List<ToolbarButtonSetting> ToolbarButtons { get; set; }

        /// <summary>
        /// 是否显示「创建二维码（分享）」按钮（位于工具栏左侧 ShareGrid）。
        /// </summary>
        public bool ShowQrCode { get; set; }

        /// <summary>
        /// 是否显示「设置」按钮（工具栏左侧 ShareGrid，与二维码同组）。
        /// 关闭后仍可从开始菜单快捷方式进入设置，不影响配置入口。
        /// </summary>
        public bool ShowSettingsButton { get; set; }

        /// <summary>是否在结束放映按钮旁显示「收起工具栏」按钮。默认 true。</summary>
        public bool ShowCollapseButton { get; set; }

		/// <summary>
		/// 批注方式：true（默认）= 使用 PPTServiceUltra.Ink 里的原版桌面批注白板窗口
		/// （压感软笔 / 手势橡皮，独立透明窗口、鼠标模式穿透、逐页笔迹），适配希沃设备；
		/// false = 使用 Office / WPS 原生批注。
		/// </summary>
		public bool UseSelfAnnotation { get; set; }

		/// <summary>
		/// 翻页按钮位置：Default（沿用程序原逻辑：WPS 两边 / Office 底部）/
		/// Bottom（强制底部两侧角）/ Both（强制左右两边）/ None（不显示翻页按钮）。
		/// </summary>
		public string PageTurningPosition { get; set; }

		/// <summary>
		/// 是否显示 PPT 加载时的转圈（圆形进度加载界面）。
		/// true（默认）= 首次分析 PPT 时显示 LoadingView；false = 跳过该加载界面。
		/// </summary>
		public bool ShowLoadingSpinner { get; set; }

		/// <summary>
		/// 界面主题：Light（默认，浅色）/ Dark（深色）/ System（跟随系统）。
		/// 由 ThemeManager 解析并下发到 Application.Resources 的主题画刷。
		/// </summary>
		public string Theme { get; set; }

		/// <summary>
		/// 放映中翻页按钮是否「自动淡化」：一段时间（约 7 秒）没有点击翻页按钮后，
		/// 左右/底部翻页按钮淡出到 0.3 透明度，直到用户再次点击才恢复不透明。
		/// true（默认）= 沿用原版自动淡化；false = 始终保持不透明。
		/// </summary>
		public bool AutoFadePageButtons { get; set; }

		/// <summary>
		/// 批注窗口层级兼容开关：
		/// false（默认）= 批注窗口作为 PPT 放映窗口的 Owner 附属窗口，只在 PPT 之上、
		/// 不进入全局置顶组（第三方窗口可正常盖住它、Win+Tab 随 PPT 分组）；
		/// true = 回退到旧版全局置顶（Topmost），批注窗口永远压在所有窗口之上。
		/// 用于个别放映环境下附属窗口层级异常时的回退。
		/// </summary>
		public bool LegacyTopmostWindow { get; set; }

		/// <summary>
		/// PPT 调用方法：Auto（默认，COM 失败时自动切兼容通道）/
		/// Com（只用注册表 COM 通道）/ Rot（只用 ROT + 后期绑定兼容通道）。
		/// </summary>
		public string PptInvokeMode { get; set; }

		public const string InvokeModeAuto = "Auto";
		public const string InvokeModeCom = "Com";
		public const string InvokeModeRot = "Rot";

		/// <summary>旧配置缺字段或值非法时回落到 Auto。</summary>
		public static string NormalizeInvokeMode(string value)
		{
			return (value == InvokeModeCom || value == InvokeModeRot) ? value : InvokeModeAuto;
		}

        /// <summary>把配置里的透明度规范到 20–100；旧配置缺字段或值非法时回落到默认 70。</summary>
        public static int NormalizeToolbarOpacity(int value)
        {
            return (value < MinToolbarOpacity || value > MaxToolbarOpacity)
                ? DefaultToolbarOpacity
                : value;
        }

		public UserConfig()
		{
			ToolbarButtons = new List<ToolbarButtonSetting>();
			ShowQrCode = true;
			ShowSettingsButton = true;
			ShowCollapseButton = true;
			PageTurningPosition = "Default";
			ShowLoadingSpinner = true;
			// 默认开启自实现批注（压感软笔 / 手势橡皮 / 板中板）。
			UseSelfAnnotation = true;
			Theme = ThemeManager.ThemeLight;
			AutoFadePageButtons = true;
			ToolbarOpacity = DefaultToolbarOpacity;
			LegacyTopmostWindow = false;
			PptInvokeMode = InvokeModeAuto;
		}
    }

    /// <summary>
    /// 用户配置读写与下发。文件位于 %APPDATA%\PPTServiceUltra\UserConfig.json。
    /// </summary>
    public static class UserConfigManager
    {
        private static readonly string ConfigDirectory = AppPath.LocalRoot();

        private static readonly string ConfigFilePath =
            Path.Combine(ConfigDirectory, "UserConfig.json");

        private static readonly object SyncRoot = new object();
        private static UserConfig _current;

        /// <summary>用户是否在设置里显式改过批注方式；改过之后 /debug 不再强制默认开启。</summary>
        private static bool _annotationUserOverride;

        /// <summary>上次下发的「翻页按钮自动淡化」值：只在开关真的变化时才刷新淡化状态，
        /// 避免实时设置时每次改动都重启 7 秒淡化计时。</summary>
        private static bool? _lastAutoFadeState;

        public static UserConfig Load()
        {
            lock (SyncRoot)
            {
                if (_current != null)
                {
                    return _current;
                }
                UserConfig cfg = null;
                try
                {
                    if (File.Exists(ConfigFilePath))
                    {
                        string json = File.ReadAllText(ConfigFilePath);
                        // 配置迁移：剔除废弃键、为旧配置补新增键默认值，并写回文件。
                        string cleanedJson;
                        if (MigrateConfigJson(json, out cleanedJson))
                        {
                            try { File.WriteAllText(ConfigFilePath, cleanedJson); } catch { }
                            // 用迁移后的 json 反序列化，确保补上的默认值在内存中生效。
                            json = cleanedJson;
                        }
                        cfg = JsonConvert.DeserializeObject<UserConfig>(json);
                    }
                }
                catch
                {
                    cfg = null;
                }
                if (cfg == null || cfg.ToolbarButtons == null || cfg.ToolbarButtons.Count == 0)
                {
                    cfg = CreateDefault();
                    Save(cfg);
                }
			// 规范化翻页位置取值，仅允许 Default / Bottom / Both / None。
			if (cfg.PageTurningPosition != "Bottom" && cfg.PageTurningPosition != "Both" && cfg.PageTurningPosition != "None")
			{
				cfg.PageTurningPosition = "Default";
			}
			// 规范化主题取值（旧配置文件可能没有该字段，保持构造函数里的浅色默认值）。
			cfg.Theme = ThemeManager.Normalize(cfg.Theme);
			// 旧配置文件没有透明度字段时保持构造函数里的 70；越界值回落到 70。
			cfg.ToolbarOpacity = UserConfig.NormalizeToolbarOpacity(cfg.ToolbarOpacity);
			// 旧配置文件没有调用方式字段时按 Auto 处理。
			cfg.PptInvokeMode = UserConfig.NormalizeInvokeMode(cfg.PptInvokeMode);
                // /debug 模式默认在内存中启用自实现批注，启动即可测试压感笔迹与手势橡皮；
                // 不写回 UserConfig.json，不影响正常启动。用户在设置里改过后以用户选择为准。
                if (DebugMode.IsEnabled && !_annotationUserOverride)
                {
                    cfg.UseSelfAnnotation = true;
                }
                _current = cfg;
                return cfg;
            }
        }

        /// <summary>
        /// 配置 json 迁移：剔除废弃键，并为旧配置补上新增键的默认值。
        /// 无变化返回 false。只动明确列出的问题键，用户其余设置保持原样。
        /// </summary>
        private static bool MigrateConfigJson(string json, out string cleanedJson)
        {
            cleanedJson = json;
            string[] obsolete =
            {
                "PageTurningMode", "WpsPageTurningPosition", "OfficePageTurningPosition",
                "PageTurningTranslucent", "HideFunctionBar"
            };
            try
            {
                JObject obj = JObject.Parse(json);
                bool changed = false;
                foreach (string key in obsolete)
                {
                    if (obj.Remove(key))
                    {
                        changed = true;
                    }
                }
                // 旧配置没有该键：bool 反序列化会得到 false，但设置按钮默认应显示。
                if (obj.Property("ShowSettingsButton") == null)
                {
                    obj["ShowSettingsButton"] = true;
                    changed = true;
                }
                if (changed)
                {
                    cleanedJson = obj.ToString(Formatting.Indented);
                }
                return changed;
            }
            catch
            {
                return false;
            }
        }

        public static UserConfig CreateDefault()
        {
            return new UserConfig
            {
                ToolbarButtons = new List<ToolbarButtonSetting>
                {
                    new ToolbarButtonSetting { Id = "Select", Visible = true },
                    new ToolbarButtonSetting { Id = "Pen", Visible = true },
                    new ToolbarButtonSetting { Id = "Rubber", Visible = true },
                    new ToolbarButtonSetting { Id = "Magnifier", Visible = true },
                    new ToolbarButtonSetting { Id = "InnerBoard", Visible = true }
                },
			ShowQrCode = true,
			ShowSettingsButton = true,
			ShowCollapseButton = true,
			PageTurningPosition = "Default",
			UseSelfAnnotation = true,
			ShowLoadingSpinner = true,
			Theme = ThemeManager.ThemeLight,
			AutoFadePageButtons = true,
			ToolbarOpacity = UserConfig.DefaultToolbarOpacity,
			LegacyTopmostWindow = false,
			PptInvokeMode = UserConfig.InvokeModeAuto
		};
        }

        public static void Save(UserConfig cfg)
        {
            if (cfg == null)
            {
                return;
            }
            lock (SyncRoot)
            {
                _annotationUserOverride = true;
                _current = cfg;
                try
                {
                    if (!Directory.Exists(ConfigDirectory))
                    {
                        Directory.CreateDirectory(ConfigDirectory);
                    }
                    string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
                    File.WriteAllText(ConfigFilePath, json);
                }
                catch
                {
                    // 忽略 IO 异常，配置仅本次内存生效。
                }
            }
        }

        /// <summary>
        /// 将当前配置下发到界面：工具栏按钮显隐/顺序、创建二维码按钮显隐、翻页按钮位置。
        /// </summary>
        public static void ApplyCurrent()
        {
            UserConfig cfg = Load();
            // 主题画刷先刷新：设置窗口与其它界面重绘时立刻取到新配色。
            ThemeManager.Apply(cfg);
            if (ViewHelper.BottomArea != null)
            {
                // 功能区/左侧按钮组的整体显隐统一由 ApplyToolbarConfig 按实际可见项决定：
                // 全部按钮都隐藏时自动收起对应区域，不再有单独的"隐藏功能栏"开关。
                ViewHelper.BottomArea.ApplyToolbarConfig(cfg);
                ViewHelper.BottomArea.ApplyCollapseButtonVisibility(cfg.ShowCollapseButton);
            }
            // 只在批注方式真的变化时重建/移除 Board：设置窗口实时生效时每次改动都会
            // 走到这里，重复 SetEnabled 会重置当前页视图并造成闪烁。
            if (AnnotationController.IsEnabled != cfg.UseSelfAnnotation)
            {
                if (cfg.UseSelfAnnotation)
                {
                    // 从 Office 原生批注切过来：工具栏可能已选中「笔/橡皮」，但
                    // AnnotationController 的模式还停留在鼠标。先按工具栏真实状态同步，
                    // 否则 SetEnabled(true) 会把 Board 切回鼠标穿透，界面是笔却画不了。
                    ViewHelper.BottomArea?.SyncModeFromToolbar();
                }
                AnnotationController.SetEnabled(cfg.UseSelfAnnotation);
                if (!cfg.UseSelfAnnotation)
                {
                    ViewHelper.BottomArea?.ResetToolbarToOfficePointer();
                }
            }
            ViewHelper.MainWindow?.ApplyPageTurningLayout();
            // 翻页按钮可能已处于淡化态：开关一改就立刻应用，不能等下次点击或下一场放映。
            if (_lastAutoFadeState != cfg.AutoFadePageButtons)
            {
                _lastAutoFadeState = cfg.AutoFadePageButtons;
                ViewHelper.MainWindow?.ApplyPageButtonFadeState(cfg.AutoFadePageButtons);
            }
            // 窗口层级兼容开关：即时切换「附属 PPT 放映窗口」/「旧版全局置顶」。
            // 内部幂等，放映未开始时仅记录状态，等 ShowBegin 再按此层级挂接。
            WindowStateHelper.ApplyWindowLayerMode(ViewHelper.MainWindow);
            // PPT 调用方式（自动 / COM / ROT）改动即时生效；放映中忽略切换，下一场放映再按新方式。
            PptOperator.Instance?.ApplyInvokeMode(cfg.PptInvokeMode);
        }

		/// <summary>
		/// 解析翻页按钮实际布局：Sides（左右两边）/ Bottom（底部两侧角）/ None（不显示翻页按钮）。
		/// Default 沿用程序原逻辑（Office 2016+ 底部，WPS/旧版 两边）。
		/// </summary>
		public static string GetPageTurningLayout()
		{
			UserConfig cfg = Load();
			if (cfg.PageTurningPosition == "Bottom")
			{
				return "Bottom";
			}
			if (cfg.PageTurningPosition == "Both")
			{
				return "Sides";
			}
			if (cfg.PageTurningPosition == "None")
			{
				return "None";
			}
			return PptVersionManager.IsOffice2016OrLater ? "Bottom" : "Sides";
		}
    }
}
