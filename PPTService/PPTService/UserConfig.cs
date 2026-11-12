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
    /// 用户配置根对象，序列化到程序启动目录 Configs/UserConfig.json。
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
        /// 翻页按钮位置模式：Auto（默认，按程序原逻辑：WPS 两边 / Office 底部）或 Custom（按下方分应用设置）。
        /// </summary>
        public string PageTurningMode { get; set; }

        /// <summary>
        /// 自定义模式下 WPS 的翻页位置：Both（两边）/ Left（仅左）/ Right（仅右）。
        /// </summary>
        public string WpsPageTurningPosition { get; set; }

        /// <summary>
        /// 自定义模式下 Office 的翻页位置：Both / Left / Right。
        /// </summary>
        public string OfficePageTurningPosition { get; set; }

        // 兼容旧版单值配置（LeftBottom / RightBottom / Both），仅用于迁移读取。
        public string PageTurningPosition { get; set; }

        public UserConfig()
        {
            ToolbarButtons = new List<ToolbarButtonSetting>();
            ShowQrCode = true;
            PageTurningMode = "Auto";
            WpsPageTurningPosition = "Both";
            OfficePageTurningPosition = "Both";
            PageTurningPosition = "Both";
        }
    }

    /// <summary>
    /// 用户配置读写与下发。文件位于程序启动目录下的 Configs/UserConfig.json。
    /// </summary>
    public static class UserConfigManager
    {
        private static readonly string ConfigDirectory =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");

        private static readonly string ConfigFilePath =
            Path.Combine(ConfigDirectory, "UserConfig.json");

        private static readonly object SyncRoot = new object();
        private static UserConfig _current;

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
                // 旧版单值翻页配置迁移到「自定义」模式
                if (string.IsNullOrEmpty(cfg.PageTurningMode)
                    && !string.IsNullOrEmpty(cfg.PageTurningPosition)
                    && cfg.PageTurningPosition != "Both")
                {
                    cfg.PageTurningMode = "Custom";
                    string mapped = MapLegacyPosition(cfg.PageTurningPosition);
                    cfg.WpsPageTurningPosition = mapped;
                    cfg.OfficePageTurningPosition = mapped;
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
                PageTurningMode = "Auto",
                WpsPageTurningPosition = "Both",
                OfficePageTurningPosition = "Both"
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
            ApplyPageTurning(cfg);
        }

        private static void ApplyPageTurning(UserConfig cfg)
        {
            string position = ResolvePosition(cfg);
            bool showLeft = position == "Both" || position == "Left";
            bool showRight = position == "Both" || position == "Right";
            SetVisible(ViewHelper.LeftBottomArea, showLeft);
            SetVisible(ViewHelper.RightBottomArea, showRight);
            SetVisible(ViewHelper.LeftArea, showLeft);
            SetVisible(ViewHelper.RightArea, showRight);
        }

        private static string ResolvePosition(UserConfig cfg)
        {
            if (cfg.PageTurningMode == "Custom")
            {
                string pos = PptVersionManager.IsWps
                    ? (cfg.WpsPageTurningPosition ?? "Both")
                    : (cfg.OfficePageTurningPosition ?? "Both");
                return pos;
            }
            if (string.IsNullOrEmpty(cfg.PageTurningMode)
                && !string.IsNullOrEmpty(cfg.PageTurningPosition))
            {
                return MapLegacyPosition(cfg.PageTurningPosition);
            }
            // Auto：按程序原逻辑（2016+/WPS 两边底部角，旧 Office 底部中央），两套布局都显示两边
            return "Both";
        }

        private static string MapLegacyPosition(string value)
        {
            if (value == "LeftBottom")
            {
                return "Left";
            }
            if (value == "RightBottom")
            {
                return "Right";
            }
            return "Both";
        }

        private static void SetVisible(UIElement area, bool visible)
        {
            if (area != null)
            {
                area.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
