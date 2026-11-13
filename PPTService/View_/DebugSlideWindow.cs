using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PPTService
{
    /// <summary>
    /// /debug 模式下的模拟幻灯片放映窗口。
    /// 它提供真实的 HWND（供 WindowStateHelper / WindowsApi / AnnotationController 使用），
    /// 并保持非 Topmost，从而始终位于工具栏窗口与批注窗口之下。
    /// 鼠标模式穿透时点击会落到本窗口，可用来验证穿透与翻页反馈。
    /// </summary>
    public class DebugSlideWindow : Window
    {
        private readonly TextBlock _pageTextBlock;

        public DebugSlideWindow()
        {
            Title = "PPTService /debug 模拟放映";
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            Topmost = false;
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = 0.0;
            Top = 0.0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Background = new SolidColorBrush(Color.FromRgb(233, 238, 245));

            Border card = new Border
            {
                Margin = new Thickness(80.0),
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(200, 210, 224)),
                BorderThickness = new Thickness(1.0),
                CornerRadius = new CornerRadius(6.0)
            };
            Grid inner = new Grid();
            inner.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
            inner.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            inner.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            TextBlock title = new TextBlock
            {
                Text = "PPT 放映模拟  /debug",
                FontSize = 40.0,
                Foreground = new SolidColorBrush(Color.FromRgb(70, 90, 120)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(title, 0);
            inner.Children.Add(title);

            _pageTextBlock = new TextBlock
            {
                FontSize = 88.0,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(40, 60, 90)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0.0, 0.0, 0.0, 12.0)
            };
            Grid.SetRow(_pageTextBlock, 1);
            inner.Children.Add(_pageTextBlock);

            TextBlock hint = new TextBlock
            {
                Text = "点击左半屏：上一页    点击右半屏：下一页    ←/→ / PageUp/PageDown：翻页    ESC：退出\n"
                    + "已默认开启「自实现批注」：笔=压感书写+颜色盘，橡皮=手势擦除/再次点击清屏，鼠标=窗口穿透",
                FontSize = 15.0,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush(Color.FromRgb(110, 125, 145)),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0.0, 0.0, 0.0, 60.0)
            };
            Grid.SetRow(hint, 2);
            inner.Children.Add(hint);

            card.Child = inner;
            Content = card;

            PptOperator.Instance.NextSlide += PptOperatorNextSlide;
            Closed += delegate
            {
                if (PptOperator.Instance != null)
                {
                    PptOperator.Instance.NextSlide -= PptOperatorNextSlide;
                }
            };
            RefreshPage();
        }

        private void PptOperatorNextSlide(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(RefreshPage));
        }

        private void RefreshPage()
        {
            _pageTextBlock.Text = string.Format("{0} / {1}",
                PptOperator.Instance.CurrentShowPosition, PptOperator.Instance.SlidesCount);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Point position = e.GetPosition(this);
            if (position.X < ActualWidth / 2.0)
            {
                PptOperator.Instance.PreviousPage();
            }
            else
            {
                PptOperator.Instance.NextPage();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                PptOperator.Instance.Exit();
            }
            else if (e.Key == Key.Left || e.Key == Key.Up || e.Key == Key.Prior)
            {
                PptOperator.Instance.PreviousPage();
            }
            else if (e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Next)
            {
                PptOperator.Instance.NextPage();
            }
        }
    }
}
