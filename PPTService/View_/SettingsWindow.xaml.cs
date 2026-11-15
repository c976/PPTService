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

        // 触控拖动排序：WPF 的 OLE 拖放（DragDrop.DoDragDrop）在触摸下不可靠，
        // 触摸路径改为直接按触点命中的行重排集合，不经过拖放。
        private bool _touchDragging;
        private ToolbarItemVm _touchDragItem;
        private Point _touchStartPoint;

        // 下拉框用「索引 <-> 配置值」映射，避免 SelectedValuePath 在内联 ComboBoxItem
        // 上偶发匹配不到、导致保存时把用户设置重置成默认值。
        private static readonly string[] ThemeValues = { "Light", "Dark", "System" };
        private static readonly string[] PagePosValues = { "Default", "Bottom", "Both", "None" };

        private static int ValueIndex(string[] values, string value)
        {
            int i = Array.IndexOf(values, value);
            return i >= 0 ? i : 0;
        }

        private static string ValueAt(string[] values, int index, string fallback)
        {
            return (index >= 0 && index < values.Length) ? values[index] : fallback;
        }

        public SettingsWindow()
        {
            InitializeComponent();
            VersionTextBlock.Text = "PPTServiceUltra 版本 " + GitVersionInfo.InformationalVersion;
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
            HideFunctionBarCheckBox.IsChecked = cfg.HideFunctionBar;
            AutoFadePageButtonsCheckBox.IsChecked = cfg.AutoFadePageButtons;
            ThemeComboBox.SelectedIndex = ValueIndex(ThemeValues, cfg.Theme);
            PageTurningPositionComboBox.SelectedIndex = ValueIndex(PagePosValues, cfg.PageTurningPosition);
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

        #region 鼠标拖拽排序

        private void ToolbarListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_touchDragging)
            {
                return;
            }
            if (IsInteractiveChild(e.OriginalSource as DependencyObject))
            {
                return;
            }
            _dragItem = GetItemFromSource(e.OriginalSource) ?? (ToolbarListBox.SelectedItem as ToolbarItemVm);
            _dragStartPoint = e.GetPosition(null);
        }

        private void ToolbarListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_touchDragging || _dragItem == null)
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

        #endregion

        #region 触控拖动排序

        private void ToolbarListBox_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (IsInteractiveChild(e.OriginalSource as DependencyObject))
            {
                _touchDragItem = null;
                return;
            }
            _touchDragItem = GetItemFromSource(e.OriginalSource) ?? (ToolbarListBox.SelectedItem as ToolbarItemVm);
            _touchStartPoint = e.GetTouchPoint(ToolbarListBox).Position;
            _touchDragging = _touchDragItem != null;
        }

        private void ToolbarListBox_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            if (!_touchDragging || _touchDragItem == null)
            {
                return;
            }
            Point pos = e.GetTouchPoint(ToolbarListBox).Position;
            if (Math.Abs(pos.Y - _touchStartPoint.Y) < 10)
            {
                return;
            }
            ToolbarItemVm target = null;
            VisualTreeHelper.HitTest(ToolbarListBox, null, delegate (HitTestResult result)
            {
                DependencyObject dep = result.VisualHit;
                while (dep != null && !(dep is ListBoxItem))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                target = (dep as ListBoxItem)?.DataContext as ToolbarItemVm;
                return HitTestResultBehavior.Stop;
            }, new PointHitTestParameters(pos));
            if (target != null && target != _touchDragItem)
            {
                int oldIndex = _items.IndexOf(_touchDragItem);
                int newIndex = _items.IndexOf(target);
                if (oldIndex >= 0 && newIndex >= 0)
                {
                    _items.Move(oldIndex, newIndex);
                    // 更新基准点，允许一次长按拖动连续跨多行。
                    _touchStartPoint = pos;
                }
            }
            e.Handled = true;
        }

        private void ToolbarListBox_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            _touchDragItem = null;
            _touchDragging = false;
        }

        #endregion

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
            cfg.HideFunctionBar = HideFunctionBarCheckBox.IsChecked == true;
            cfg.AutoFadePageButtons = AutoFadePageButtonsCheckBox.IsChecked == true;
            cfg.Theme = ValueAt(ThemeValues, ThemeComboBox.SelectedIndex, ThemeManager.ThemeLight);
            cfg.PageTurningPosition = ValueAt(PagePosValues, PageTurningPositionComboBox.SelectedIndex, "Default");
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
            HideFunctionBarCheckBox.IsChecked = def.HideFunctionBar;
            AutoFadePageButtonsCheckBox.IsChecked = def.AutoFadePageButtons;
            ThemeComboBox.SelectedIndex = ValueIndex(ThemeValues, def.Theme);
            PageTurningPositionComboBox.SelectedIndex = ValueIndex(PagePosValues, def.PageTurningPosition);
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
