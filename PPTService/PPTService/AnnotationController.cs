using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace PPTService
{
    /// <summary>
    /// 批注窗口控制器：把 PPT 小工具的工具栏（鼠标 / 笔 / 橡皮 / 颜色）与 PPT 翻页事件
    /// 接到 PPTService.Ink 里的原版桌面批注白板窗口（AnnotationWindow）。
    ///
    /// 与原版桌面批注一致的能力：
    ///  - 压感软笔：DefaultDesktopBoardModesProvider 的 SoftPenInputFromBoardWritingMode；
    ///  - 手势橡皮：EraserMode + EraserGestureFilter（Initializer 里注册）；
    ///  - 鼠标模式：SetTransparentHitThrough 设置 WS_EX_TRANSPARENT，点击穿透到 PPT；
    ///  - 逐页笔迹：GoToSlide，PPT 翻页（含不通过小工具的翻页）时同步切换。
    /// </summary>
    public static class AnnotationController
    {
        public enum Mode
        {
            Mouse,
            Pen,
            Eraser
        }

        private const uint SwpNosize = 0x0001;
        private const uint SwpNomove = 0x0002;
        private const uint SwpNoactivate = 0x0010;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, uint uFlags);

        private static AnnotationWindow _window;
        private static Window _toolbarWindow;
        private static int _currentPageIndex = 1;
        private static Color _color = Color.FromRgb(242, 24, 24);

        public static bool IsEnabled { get; private set; }

        public static Mode CurrentMode { get; private set; }

        /// <summary>
        /// 传入小工具主窗口：批注窗口始终放在它之下，保证工具栏与翻页按钮可点击。
        /// </summary>
        public static void Attach(Window toolbarWindow)
        {
            _toolbarWindow = toolbarWindow;
            if (_toolbarWindow == null)
            {
                return;
            }
            _toolbarWindow.Activated += delegate { KeepBelowToolbar(); };
            _toolbarWindow.Closed += delegate { Close(); };
            try
            {
                _currentPageIndex = Math.Max(1, PptOperator.Instance.CurrentShowPosition);
            }
            catch
            {
                _currentPageIndex = 1;
            }
        }

        private static AnnotationWindow EnsureWindow()
        {
            if (_window != null && !_window.IsClosing)
            {
                return _window;
            }
            AnnotationWindow window = new AnnotationWindow();
            window.SetInkColor(_color);
            // 让原版 FullScreenHelper 在 Loaded 时把窗口铺到放映所在的显示器：
            // 先按幻灯片放映窗口的位置初始化一次，再 Show()。
            ApplyInitialBounds(window);
            window.Closed += delegate
            {
                if (_window == window)
                {
                    _window = null;
                }
            };
            _window = window;
            return window;
        }

        private static void ApplyInitialBounds(Window window)
        {
            WindowRect rect;
            IntPtr slideHwnd = IntPtr.Zero;
            try
            {
                slideHwnd = PptOperator.Instance.SlideShowWindowIntPtr;
            }
            catch
            {
                // 放映尚未开始，回退到主窗口 / 主屏。
            }
            double factor = ScreenHelper.DpiFactor;
            if (factor <= 0.0)
            {
                factor = 1.0;
            }
            if (slideHwnd != IntPtr.Zero &&
                WindowsApi.GetWindowRect(new HandleRef(window, slideHwnd), out rect) &&
                rect.Right > rect.Left && rect.Bottom > rect.Top)
            {
                window.Left = rect.Left / factor;
                window.Top = rect.Top / factor;
                window.Width = (rect.Right - rect.Left) / factor;
                window.Height = (rect.Bottom - rect.Top) / factor;
                return;
            }
            if (_toolbarWindow != null)
            {
                window.Left = _toolbarWindow.Left;
                window.Top = _toolbarWindow.Top;
                window.Width = _toolbarWindow.Width;
                window.Height = _toolbarWindow.Height;
                return;
            }
            window.Left = 0.0;
            window.Top = 0.0;
            window.Width = SystemParameters.PrimaryScreenWidth;
            window.Height = SystemParameters.PrimaryScreenHeight;
        }

        public static void SetEnabled(bool enable)
        {
            IsEnabled = enable;
            if (!enable)
            {
                Hide();
                return;
            }
            // 自实现批注启用时，Office/WPS 自身的指针必须回到「无」，
            // 否则鼠标模式下点击穿透到 PPT 会同时触发 Office 原生画笔。
            try
            {
                PptOperator.Instance.PointerType =
                    Microsoft.Office.Interop.PowerPoint.PpSlideShowPointerType.ppSlideShowPointerNone;
            }
            catch
            {
                // 放映未开始，忽略。
            }
            AnnotationWindow window = EnsureWindow();
            if (!window.IsVisible)
            {
                window.Show();
            }
            KeepBelowToolbar();
            window.GoToSlide(_currentPageIndex - 1);
            SetMode(CurrentMode);
        }

        /// <summary>放映结束只隐藏窗口，不改变用户的批注方式配置。</summary>
        public static void Hide()
        {
            if (_window != null && _window.IsVisible)
            {
                _window.Hide();
            }
        }

        public static void Close()
        {
            AnnotationWindow window = _window;
            _window = null;
            if (window != null && !window.IsClosing)
            {
                window.Close();
            }
        }

        public static void SetMode(Mode mode)
        {
            CurrentMode = mode;
            if (!IsEnabled || _window == null || _window.IsClosing)
            {
                return;
            }
            if (mode == Mode.Mouse)
            {
                _window.GotoSelect();
            }
            else if (mode == Mode.Pen)
            {
                _window.GotoInk();
            }
            else
            {
                _window.GotoEraser();
            }
            KeepBelowToolbar();
        }

        public static void SetColor(Color color)
        {
            _color = color;
            if (_window != null && !_window.IsClosing)
            {
                _window.SetInkColor(color);
            }
        }

        /// <summary>清除当前页全部笔迹（对应桌面批注橡皮弹窗的滑杆清屏）。</summary>
        public static void ClearCurrentPage()
        {
            if (_window != null && !_window.IsClosing)
            {
                _window.CleanInk();
            }
        }

        /// <summary>PPT 翻页（含不通过小工具的翻页）时切换到对应页的笔迹。</summary>
        public static void ShowPage(int pageIndex)
        {
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            _currentPageIndex = pageIndex;
            if (!IsEnabled || _window == null || _window.IsClosing)
            {
                return;
            }
            _window.GoToSlide(pageIndex - 1);
        }

        /// <summary>
        /// 把批注窗口插到小工具主窗口之后（z 序紧随其后、位于其下）。
        /// 两个窗口都是 Topmost，这样工具栏 / 翻页按钮可点，空白处的输入落到白板。
        /// </summary>
        public static void KeepBelowToolbar()
        {
            if (_window == null || _toolbarWindow == null)
            {
                return;
            }
            IntPtr hwnd = new WindowInteropHelper(_window).Handle;
            IntPtr toolbar = new WindowInteropHelper(_toolbarWindow).Handle;
            if (hwnd == IntPtr.Zero || toolbar == IntPtr.Zero || hwnd == toolbar)
            {
                return;
            }
            SetWindowPos(hwnd, toolbar, 0, 0, 0, 0,
                SwpNomove | SwpNosize | SwpNoactivate);
        }
    }
}
