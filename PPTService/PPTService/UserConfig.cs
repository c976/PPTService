using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using PPTService.Collection;

namespace PPTService
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
    /// 用户配置根对象，序列化到 %LOCALAPPDATA%\PPTServiceUltra\UserConfig.json。
    /// </summary>
    public class UserConfig
    {
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
		/// 批注方式：true（默认）= 使用 PPTService.Ink 里的原版桌面批注白板窗口
		/// （压感软笔 / 手势橡皮，独立透明窗口、鼠标模式穿透、逐页笔迹），适配希沃设备；
		/// false = 使用 Office / WPS 原生批注。
		/// </summary>
		public bool UseSelfAnnotation { get; set; }

		/// <summary>
		/// 翻页按钮位置：Default（沿用程序原逻辑：WPS 两边 / Office 底部）/
		/// Bottom（强制底部两侧角）/ Both（强制左右两边）。
		/// </summary>
		public string PageTurningPosition { get; set; }

		/// <summary>
		/// 是否显示 PPT 加载时的转圈（圆形进度加载界面）。
		/// true（默认）= 首次分析 PPT 时显示 LoadingView；false = 跳过该加载界面。
		/// </summary>
		public bool ShowLoadingSpinner { get; set; }

		// 兼容旧版配置字段（Auto/Custom/Wps/Office），反序列化时忽略，不再使用。
        public string PageTurningMode { get; set; }
        public string WpsPageTurningPosition { get; set; }
        public string OfficePageTurningPosition { get; set; }

		public UserConfig()
		{
			ToolbarButtons = new List<ToolbarButtonSetting>();
			ShowQrCode = true;
			PageTurningPosition = "Default";
			ShowLoadingSpinner = true;
			// 默认开启自实现批注（压感软笔 / 手势橡皮 / 板中板）。
			UseSelfAnnotation = true;
		}
    }

    /// <summary>
    /// 用户配置读写与下发。文件位于 %LOCALAPPDATA%\PPTServiceUltra\UserConfig.json。
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
                // 规范化翻页位置取值，仅允许 Default / Bottom / Both。
                if (cfg.PageTurningPosition != "Bottom" && cfg.PageTurningPosition != "Both")
                {
                    cfg.PageTurningPosition = "Default";
                }
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
			PageTurningPosition = "Default",
			UseSelfAnnotation = true,
			ShowLoadingSpinner = true
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
            if (ViewHelper.BottomArea != null)
            {
                ViewHelper.BottomArea.ApplyToolbarConfig(cfg);
            }
            AnnotationController.SetEnabled(cfg.UseSelfAnnotation);
            ViewHelper.MainWindow?.ApplyPageTurningLayout();
        }

        /// <summary>
        /// 解析翻页按钮实际布局：Sides（左右两边）/ Bottom（底部两侧角）。
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
            return PptVersionManager.IsOffice2016OrLater ? "Bottom" : "Sides";
        }
    }
}
