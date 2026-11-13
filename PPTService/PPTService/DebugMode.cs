using System;
using System.Globalization;
using System.Linq;
using System.Windows.Interop;

namespace PPTService
{
    /// <summary>
    /// /debug 模拟放映模式：在没有安装 Office / WPS 的机器上，
    /// 用一个模拟幻灯片窗口提供真实的 SlideShowWindowIntPtr，
    /// 并让 PptOperator 返回纯内存状态，从而完整显示放映时的工具栏、
    /// 翻页按钮与自实现批注窗口。
    /// </summary>
    public static class DebugMode
    {
        /// <summary>是否处于模拟放映模式。</summary>
        public static bool IsEnabled { get; private set; }

        /// <summary>模拟的幻灯片总页数，可用 /debug:20 指定。</summary>
        public static int SlidesCount { get; private set; } = 12;

        /// <summary>模拟放映窗口（充当 PowerPoint SlideShowWindow）。</summary>
        public static DebugSlideWindow SlideWindow { get; private set; }

        public static bool IsDebugArg(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return false;
            }
            string text = arg.Trim();
            return text.StartsWith("/debug", StringComparison.OrdinalIgnoreCase)
                || text.StartsWith("--debug", StringComparison.OrdinalIgnoreCase)
                || text.StartsWith("-debug", StringComparison.OrdinalIgnoreCase);
        }

        public static void Enable(string[] args)
        {
            if (args == null)
            {
                return;
            }
            string arg = args.FirstOrDefault(IsDebugArg);
            if (arg == null)
            {
                return;
            }
            IsEnabled = true;
            int colon = arg.IndexOf(':');
            if (colon > 0)
            {
                int count;
                if (int.TryParse(arg.Substring(colon + 1), NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out count) && count >= 1 && count <= 500)
                {
                    SlidesCount = count;
                }
            }
        }

        /// <summary>
        /// 创建并显示模拟放映窗口，把它的 HWND 交给 PptOperator。
        /// 必须在 PptOperator.Initialize() 之前调用。
        /// </summary>
        public static void CreateSlideWindow()
        {
            SlideWindow = new DebugSlideWindow();
            SlideWindow.Show();
            IntPtr hwnd = new WindowInteropHelper(SlideWindow).EnsureHandle();
            PptOperator.Instance.SetDebugSlideWindowHandle(hwnd);
        }
    }
}
