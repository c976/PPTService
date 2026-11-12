using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

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
        /// 工具栏功能按钮列表，顺序即显示顺序；Visible 控制显隐。
        /// 仅包含这 5 个功能按钮：Select / Pen / Rubber / Magnifier / InnerBoard。
        /// </summary>
        public List<ToolbarButtonSetting> ToolbarButtons { get; set; }

        /// <summary>
        /// 翻页按钮位置：LeftBottom（左下角）/ RightBottom（右下角）/ Both（两边，默认）。
        /// </summary>
        public string PageTurningPosition { get; set; }

        public UserConfig()
        {
            ToolbarButtons = new List<ToolbarButtonSetting>();
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
                PageTurningPosition = "Both"
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
        /// 将当前配置下发到界面：工具栏按钮显隐/顺序、翻页按钮位置。
        /// </summary>
        public static void ApplyCurrent()
        {
            UserConfig cfg = Load();
            if (ViewHelper.BottomArea != null)
            {
                ViewHelper.BottomArea.ApplyToolbarConfig(cfg);
            }
            if (cfg.PageTurningPosition == "LeftBottom")
            {
                if (ViewHelper.LeftBottomArea != null)
                {
                    ViewHelper.LeftBottomArea.Visibility = Visibility.Visible;
                }
                if (ViewHelper.RightBottomArea != null)
                {
                    ViewHelper.RightBottomArea.Visibility = Visibility.Collapsed;
                }
            }
            else if (cfg.PageTurningPosition == "RightBottom")
            {
                if (ViewHelper.RightBottomArea != null)
                {
                    ViewHelper.RightBottomArea.Visibility = Visibility.Visible;
                }
                if (ViewHelper.LeftBottomArea != null)
                {
                    ViewHelper.LeftBottomArea.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (ViewHelper.LeftBottomArea != null)
                {
                    ViewHelper.LeftBottomArea.Visibility = Visibility.Visible;
                }
                if (ViewHelper.RightBottomArea != null)
                {
                    ViewHelper.RightBottomArea.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
