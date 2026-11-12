using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace PPTService
{
    public partial class SettingsWindow : Window
    {
        private ObservableCollection<ToolbarItemVm> _items;

        public SettingsWindow()
        {
            InitializeComponent();
            VersionTextBlock.Text = "版本 " + GitVersionInfo.InformationalVersion;
            LoadFromConfig();
        }

        private void LoadFromConfig()
        {
            UserConfig cfg = UserConfigManager.Load();
            _items = new ObservableCollection<ToolbarItemVm>();
            foreach (ToolbarButtonSetting b in cfg.ToolbarButtons)
            {
                _items.Add(new ToolbarItemVm { Id = b.Id, Name = ToolbarName(b.Id), IsVisible = b.Visible });
            }
            ToolbarListBox.ItemsSource = _items;
            if (cfg.PageTurningPosition == "LeftBottom")
            {
                RbLeft.IsChecked = true;
            }
            else if (cfg.PageTurningPosition == "RightBottom")
            {
                RbRight.IsChecked = true;
            }
            else
            {
                RbBoth.IsChecked = true;
            }
        }

        private static string ToolbarName(string id)
        {
            switch (id)
            {
                case "Select": return "选择";
                case "Pen": return "画笔";
                case "Rubber": return "橡皮";
                case "Magnifier": return "放大镜";
                case "InnerBoard": return "小黑板";
                default: return id;
            }
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            ToolbarItemVm vm = (sender as Button)?.Tag as ToolbarItemVm;
            if (vm == null)
            {
                return;
            }
            int i = _items.IndexOf(vm);
            if (i > 0)
            {
                _items.Move(i, i - 1);
            }
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            ToolbarItemVm vm = (sender as Button)?.Tag as ToolbarItemVm;
            if (vm == null)
            {
                return;
            }
            int i = _items.IndexOf(vm);
            if (i >= 0 && i < _items.Count - 1)
            {
                _items.Move(i, i + 1);
            }
        }

        private UserConfig BuildConfig()
        {
            UserConfig cfg = new UserConfig();
            foreach (ToolbarItemVm vm in _items)
            {
                cfg.ToolbarButtons.Add(new ToolbarButtonSetting { Id = vm.Id, Visible = vm.IsVisible });
            }
            if (RbLeft.IsChecked == true)
            {
                cfg.PageTurningPosition = "LeftBottom";
            }
            else if (RbRight.IsChecked == true)
            {
                cfg.PageTurningPosition = "RightBottom";
            }
            else
            {
                cfg.PageTurningPosition = "Both";
            }
            return cfg;
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            UserConfig cfg = BuildConfig();
            UserConfigManager.Save(cfg);
            UserConfigManager.ApplyCurrent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Apply_Click(sender, e);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            UserConfig def = UserConfigManager.CreateDefault();
            _items.Clear();
            foreach (ToolbarButtonSetting b in def.ToolbarButtons)
            {
                _items.Add(new ToolbarItemVm { Id = b.Id, Name = ToolbarName(b.Id), IsVisible = b.Visible });
            }
            RbBoth.IsChecked = true;
        }
    }

    public class ToolbarItemVm : INotifyPropertyChanged
    {
        private bool _isVisible;

        public string Id { get; set; }

        public string Name { get; set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
