using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PPTService
{
    public partial class SettingsWindow : Window
    {
        private ObservableCollection<ToolbarItemVm> _items;

        private ToolbarItemVm _dragItem;
        private Point _dragStartPoint;

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
            QrCodeCheckBox.IsChecked = cfg.ShowQrCode;
            ShowLoadingCheckBox.IsChecked = cfg.ShowLoadingSpinner;
            EnableSelfAnnotationCheckBox.IsChecked = cfg.UseSelfAnnotation;
            if (cfg.PageTurningPosition == "Bottom")
            {
                RbBottom.IsChecked = true;
            }
            else if (cfg.PageTurningPosition == "Both")
            {
                RbBoth.IsChecked = true;
            }
            else
            {
                RbDefault.IsChecked = true;
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
                case "InnerBoard": return "板中板";
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

        #region 拖拽排序

        private void ToolbarListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsInteractiveChild(e.OriginalSource as DependencyObject))
            {
                return;
            }
            _dragItem = GetItemFromSource(e.OriginalSource) ?? (ToolbarListBox.SelectedItem as ToolbarItemVm);
            _dragStartPoint = e.GetPosition(null);
        }

        private void ToolbarListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragItem == null)
            {
                return;
            }
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                _dragItem = null;
                return;
            }
            Point pos = e.GetPosition(null);
            if (Math.Abs(pos.X - _dragStartPoint.X) > 5 || Math.Abs(pos.Y - _dragStartPoint.Y) > 5)
            {
                DragDrop.DoDragDrop(ToolbarListBox, _dragItem, DragDropEffects.Move);
                _dragItem = null;
            }
        }

        private void ToolbarListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void ToolbarListBox_Drop(object sender, DragEventArgs e)
        {
            ToolbarItemVm target = GetItemFromSource(e.OriginalSource);
            if (_dragItem != null && target != null && target != _dragItem)
            {
                int oldIndex = _items.IndexOf(_dragItem);
                int newIndex = _items.IndexOf(target);
                if (oldIndex >= 0 && newIndex >= 0)
                {
                    _items.Move(oldIndex, newIndex);
                }
            }
            _dragItem = null;
            e.Handled = true;
        }

        private ToolbarItemVm GetItemFromSource(object originalSource)
        {
            DependencyObject dep = originalSource as DependencyObject;
            while (dep != null && !(dep is ListBoxItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }
            ListBoxItem item = dep as ListBoxItem;
            if (item != null)
            {
                return item.DataContext as ToolbarItemVm;
            }
            return null;
        }

        private static bool IsInteractiveChild(DependencyObject dep)
        {
            while (dep != null)
            {
                if (dep is CheckBox || dep is Button)
                {
                    return true;
                }
                dep = VisualTreeHelper.GetParent(dep);
            }
            return false;
        }

        #endregion

        private UserConfig BuildConfig()
        {
            UserConfig cfg = new UserConfig();
            foreach (ToolbarItemVm vm in _items)
            {
                cfg.ToolbarButtons.Add(new ToolbarButtonSetting { Id = vm.Id, Visible = vm.IsVisible });
            }
            cfg.ShowQrCode = QrCodeCheckBox.IsChecked == true;
            cfg.ShowLoadingSpinner = ShowLoadingCheckBox.IsChecked == true;
            cfg.UseSelfAnnotation = EnableSelfAnnotationCheckBox.IsChecked == true;
            if (RbBottom.IsChecked == true)
            {
                cfg.PageTurningPosition = "Bottom";
            }
            else if (RbBoth.IsChecked == true)
            {
                cfg.PageTurningPosition = "Both";
            }
            else
            {
                cfg.PageTurningPosition = "Default";
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
            QrCodeCheckBox.IsChecked = def.ShowQrCode;
            ShowLoadingCheckBox.IsChecked = def.ShowLoadingSpinner;
            EnableSelfAnnotationCheckBox.IsChecked = def.UseSelfAnnotation;
            RbDefault.IsChecked = true;
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
