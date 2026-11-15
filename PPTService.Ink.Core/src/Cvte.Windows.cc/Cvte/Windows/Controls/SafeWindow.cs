using System;
using System.ComponentModel;
using System.Windows;
using PPTService.Ink.Core.ComponentModel;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.Controls
{
	[PublicAPI]
	public class SafeWindow : Window
	{
		private bool _isClosed = true;

		public SafeWindow()
		{
			Activated += SafeWindow_Activated;
			Closing += SafeWindow_Closing;
			Loaded += SafeWindow_Loaded;
		}

		private void SafeWindow_Loaded(object sender, RoutedEventArgs e)
		{
			if (WindowStyle == WindowStyle.None && ResizeMode != ResizeMode.NoResize)
			{
				throw new InvalidOperationException("安全窗口检测到可能出现问题的窗口属性值。在设置 WindowStyle 为 None 时，请同时设置 ResizeMode 为 NoResize。详见：https://kb.cvte.com/pages/viewpage.action?pageId=174513270 此异常仅调试抛出。异常窗口为：" + GetType().FullName);
			}
			_isClosed = false;
		}

		private void SafeWindow_Closing(object sender, CancelEventArgs e)
		{
			if (DebuggingProperties.IsDebug)
			{
				if (!_isClosed)
				{
					throw new InvalidOperationException("请使用 SafeWindow.Close() 方法关闭，以避免 Win32 窗口关闭时可能出现的若干问题。详见：https://lindexi.github.io/lindexi/post/WPF-%E8%A7%A3%E5%86%B3%E5%BC%B9%E5%87%BA%E6%A8%A1%E6%80%81%E7%AA%97%E5%8F%A3%E5%85%B3%E9%97%AD%E5%90%8E-%E4%B8%BB%E7%AA%97%E5%8F%A3%E4%B8%8D%E5%9C%A8%E6%9C%80%E5%89%8D.html 和 https://walterlv.github.io/post/fix-owner-window-dropping-down-when-close-a-modal-child-window.html 此异常仅调试抛出。异常窗口为：{this.GetType().FullName}");
				}
			}
			else if (!_isClosed)
			{
				e.Cancel = true;
				Dispatcher.InvokeAsync(Close);
			}
		}

		private void SafeWindow_Activated(object sender, EventArgs e)
		{
			_isClosed = false;
		}

		public new void Close()
		{
			_isClosed = true;
			ActiveOwnerWindow();
			base.Close();
		}

		private void ActiveOwnerWindow()
		{
			Owner?.Activate();
		}
	}
}
