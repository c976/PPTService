using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Cvte.Paint.DesktopMode;
using Cvte.Paint.Framework;

namespace PPTService
{
	/// <summary>
	/// 批注窗口。直接继承桌面批注（net45）原版白板窗口 DesktopBoardWindow，
	/// 原样保留其 InkBoard 白板、压感软笔（SoftPenInputFromBoardWritingMode）、
	/// 手势橡皮（EraserMode）、逐页独立笔迹（GoToSlide）。
	/// </summary>
	public class AnnotationWindow : DesktopBoardWindow
	{
		private const int GwlExstyle = -20;
		private const int WsExTransparent = 0x20;

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		public AnnotationWindow()
		{
			ShowInTaskbar = false;
			// 笔/橡皮模式下窗口必须可命中。原版分层窗口分支里 "#0100000" 少一位，
			// 会把 Background 写成无效值导致整窗不接收鼠标，这里显式给 alpha=1 的背景。
			Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
			// 主题 BAML 未被嵌入（或资源名不匹配）时 Board 没有模板，
			// 输入层 BoardEventSource 不存在，会表现为「怎么都画不出笔迹」。
			EnsureBoardStyle();
			// 初始为鼠标模式：窗口穿透，点击落到 PPT。
			GotoSelect();
			Loaded += AnnotationWindow_Loaded;
			// 注意：不要在 Activated 里做任何 SetWindowPos。书写时窗口会被激活，
			// 跨窗口 z 序操作与 WPF 触笔线程互相等待会造成「画第一笔就卡死」。
		}

		private void AnnotationWindow_Loaded(object sender, RoutedEventArgs e)
		{
			// 模板应用之后再确认一次输入层确实存在。
			if (FindChild<BoardEventSource>(InkBoard) == null)
			{
				InkBoardStyleFallback.Apply(InkBoard);
			}
			AnnotationController.KeepBelowToolbar();
		}

		private void EnsureBoardStyle()
		{
			try
			{
				if (InkBoard.TryFindResource(typeof(Board)) == null)
				{
					InkBoardStyleFallback.Apply(InkBoard);
				}
			}
			catch
			{
				// 主题字典加载异常（例如 pack URI 大小写不匹配）时也用代码模板兜底。
				InkBoardStyleFallback.Apply(InkBoard);
			}
		}

		/// <summary>
		/// 显式控制鼠标穿透，不依赖 PerformanceDesktopTransparentWindow 对
		/// DWM / 分层窗口两条分支的判断。
		/// </summary>
		public void SetHitThrough(bool through)
		{
			IntPtr hwnd = new WindowInteropHelper(this).EnsureHandle();
			int style = GetWindowLong(hwnd, GwlExstyle);
			if (through)
			{
				style |= WsExTransparent;
			}
			else
			{
				style &= ~WsExTransparent;
			}
			SetWindowLong(hwnd, GwlExstyle, style);
			Background = through
				? Brushes.Transparent
				: new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));
		}

		private static T FindChild<T>(DependencyObject parent) where T : DependencyObject
		{
			if (parent == null)
			{
				return null;
			}
			int count = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < count; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				T match = child as T;
				if (match != null)
				{
					return match;
				}
				T nested = FindChild<T>(child);
				if (nested != null)
				{
					return nested;
				}
			}
			return null;
		}
	}
}
