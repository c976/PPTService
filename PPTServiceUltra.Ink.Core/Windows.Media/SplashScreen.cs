using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using PPTServiceUltra.Ink.Core.ComponentModel;
using PPTServiceUltra.Ink.Core.Windows.Native;

namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	public class SplashScreen
	{
		private class StringToIntPtrMarshaler : IDisposable
		{
			private readonly IntPtr _ptr;

			private bool _isDisposed;

			public StringToIntPtrMarshaler(string @string)
			{
				_ptr = Marshal.StringToHGlobalUni(@string);
			}

			public IntPtr GetPtr()
			{
				return _ptr;
			}

			~StringToIntPtrMarshaler()
			{
				Dispose(false);
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool _)
			{
				if (!_isDisposed)
				{
					Marshal.FreeHGlobal(_ptr);
				}
				_isDisposed = true;
			}
		}

		private readonly string _splashFile;

		private readonly string _windowClass;

		private readonly string _windowName;

		private readonly bool _topmost;

		private int _imageWidth;

		private int _imageHeight;

		private bool _requestClose;

		private volatile int _isRunning;

		private IntPtr _window;

		private Win32.Rectangle _windowRectangle;

		private Win32.Size _windowSize;

		private readonly IntPtr _screenDC = Win32.User32.GetDC(IntPtr.Zero);

		private IntPtr _windowDC;

		private IntPtr _memoryDC = IntPtr.Zero;

		private IntPtr _splashImage;

		private IntPtr _graphics;

		private IntPtr _memoryBitmap = IntPtr.Zero;

		private UIntPtr _gdipToken = UIntPtr.Zero;

		private IntPtr _timer;

		public SplashScreen(FileInfo file, int? width = null, int? height = null, string windowTitle = null, bool topmost = false)
		{
			if (file == null)
			{
				throw new ArgumentNullException("file");
			}
			if (width.HasValue && width <= 0)
			{
				throw new ArgumentOutOfRangeException("width", "图片的宽度必须为非负值。");
			}
			if (height.HasValue && height <= 0)
			{
				throw new ArgumentOutOfRangeException("height", "图片的高度必须为非负值。");
			}
			if (DebuggingProperties.IsDebug && !file.Exists)
			{
				throw new FileNotFoundException("启动图文件不存在：" + file.FullName + "。");
			}
			_splashFile = file.FullName;
			_imageWidth = width.GetValueOrDefault();
			_imageHeight = height.GetValueOrDefault();
			_windowClass = windowTitle ?? "Splash Image";
			_windowName = windowTitle ?? "Splash Image";
			_topmost = topmost;
		}

		public Task ShowAsync()
		{
			return Task.Run((Action)Show);
		}

		private void Show()
		{
			if (Interlocked.CompareExchange(ref _isRunning, 1, 0) != 0)
			{
				return;
			}
			try
			{
				ShowCore();
			}
			catch (Win32Exception)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw;
				}
			}
			finally
			{
				_isRunning = 0;
			}
		}

		public void Close()
		{
			if (_window != IntPtr.Zero)
			{
				Win32.User32.PostMessage(_window, 16u, IntPtr.Zero, IntPtr.Zero);
			}
			_requestClose = true;
		}

		private void ShowCore()
		{
			try
			{
				IntPtr handle = Process.GetCurrentProcess().Handle;
				using (StringToIntPtrMarshaler stringToIntPtrMarshaler = new StringToIntPtrMarshaler(_windowClass))
				{
					Win32.WindowClassExPtr lpwcx = new Win32.WindowClassExPtr
					{
						Size = (uint)Marshal.SizeOf(typeof(Win32.WindowClassExPtr)),
						Styles = Win32.WindowClassStyles.CS_DBLCLKS,
						WindowProc = WindowProc,
						ClassExtraBytes = 0,
						WindowExtraBytes = 0,
						InstanceHandle = handle,
						IconHandle = Win32.User32.LoadIcon(IntPtr.Zero, Win32.SystemIcon.IDI_APPLICATION),
						CursorHandle = Win32.User32.LoadCursor(IntPtr.Zero, Win32.SystemCursor.IDC_ARROW),
						BackgroundBrushHandle = (IntPtr)5,
						MenuName = IntPtr.Zero,
						ClassName = stringToIntPtrMarshaler.GetPtr()
					};
					if (Win32.User32.RegisterClassEx(ref lpwcx) != 0)
					{
						_window = Win32.User32.CreateWindowEx(Win32.WindowExStyles.WS_EX_LAYERED, _windowClass, _windowName, Win32.WindowStyles.WS_OVERLAPPED, int.MinValue, int.MinValue, int.MinValue, int.MinValue, IntPtr.Zero, IntPtr.Zero, handle, IntPtr.Zero);
						if (_window != IntPtr.Zero)
						{
							_windowDC = Win32.User32.GetDC(_window);
							Win32.GdiplusStartupInput input = new Win32.GdiplusStartupInput
							{
								GdiplusVersion = 1u,
								DebugEventCallback = IntPtr.Zero,
								SuppressBackgroundThread = false,
								SuppressExternalCodecs = false
							};
							Win32.GdiplusStartupOutput output;
							if (Win32.GdiPlus.GdiplusStartup(out _gdipToken, ref input, out output) != Win32.GpStatus.Ok || Win32.GdiPlus.GdipLoadImageFromFile(_splashFile, out _splashImage) != Win32.GpStatus.Ok)
							{
								return;
							}
							if (_imageHeight == 0)
							{
								uint height;
								if (Win32.GdiPlus.GdipGetImageHeight(_splashImage, out height) == Win32.GpStatus.Ok)
								{
									_imageHeight = (int)height;
								}
								else
								{
									_imageHeight = 1;
								}
							}
							if (_imageWidth == 0)
							{
								uint width;
								if (Win32.GdiPlus.GdipGetImageWidth(_splashImage, out width) == Win32.GpStatus.Ok)
								{
									_imageWidth = (int)width;
								}
								else
								{
									_imageWidth = 1;
								}
							}
							SetPositionAndSize();
							Win32.User32.ShowWindow(_window, Win32.ShowWindowCommands.SW_SHOWNORMAL);
							DrawImage();
							_timer = Win32.User32.SetTimer(_window, IntPtr.Zero, 200u, null);
							Win32.Message lpMsg;
							while (Win32.User32.GetMessage(out lpMsg, IntPtr.Zero, 0u, 0u) != 0)
							{
								if (!_requestClose)
								{
									Win32.User32.TranslateMessage(ref lpMsg);
									Win32.User32.DispatchMessage(ref lpMsg);
								}
								else
								{
									Win32.User32.DestroyWindow(_window);
								}
							}
							return;
						}
						throw new Win32Exception(Marshal.GetLastWin32Error());
					}
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
			finally
			{
				ReleaseAllResource();
			}
		}

		private void SetPositionAndSize(int? dpi = null)
		{
			int num = 0;
			int num2 = 0;
			int num3 = Win32.User32.GetSystemMetrics(Win32.SystemMetrics.SM_CXSCREEN);
			int num4 = Win32.User32.GetSystemMetrics(Win32.SystemMetrics.SM_CYSCREEN);
			IntPtr hMonitor = Win32.User32.MonitorFromWindow(_window, Win32.MonitorFlag.MONITOR_DEFAULTTONULL);
			Win32.MonitorInfo lpmi = default(Win32.MonitorInfo);
			lpmi.Size = (uint)Marshal.SizeOf((object)lpmi);
			if (Win32.User32.GetMonitorInfo(hMonitor, ref lpmi))
			{
				num = lpmi.MonitorRect.Left;
				num2 = lpmi.MonitorRect.Top;
				num3 = lpmi.MonitorRect.Right - lpmi.MonitorRect.Left;
				num4 = lpmi.MonitorRect.Bottom - lpmi.MonitorRect.Top;
			}
			if (!dpi.HasValue)
			{
				dpi = ((!(Environment.OSVersion.Version > new Version(10, 0, 1607))) ? new int?(Win32.Gdi32.GetDeviceCaps(_windowDC, Win32.DeviceCapIndexes.LOGPIXELSX)) : new int?((int)Win32.User32.GetDpiForWindow(_window)));
			}
			int num5 = _imageWidth * dpi.Value / 96;
			int num6 = _imageHeight * dpi.Value / 96;
			Win32.HwndZOrder hwndZOrder = (_topmost ? Win32.HwndZOrder.HWND_TOPMOST : Win32.HwndZOrder.HWND_NOTOPMOST);
			Win32.User32.SetWindowPos(_window, (IntPtr)(int)hwndZOrder, (num3 - num5) / 2 + num, (num4 - num6) / 2 + num2, num5, num6, 0);
			Win32.User32.GetWindowRect(_window, out _windowRectangle);
			_windowSize = new Win32.Size
			{
				Width = _windowRectangle.Right - _windowRectangle.Left,
				Height = _windowRectangle.Bottom - _windowRectangle.Top
			};
		}

		private void DrawImage()
		{
			if (_memoryDC != IntPtr.Zero)
			{
				Win32.Gdi32.DeleteDC(_memoryDC);
			}
			if (_memoryBitmap != IntPtr.Zero)
			{
				Win32.Gdi32.DeleteObject(_memoryBitmap);
			}
			_memoryDC = Win32.Gdi32.CreateCompatibleDC(_windowDC);
			_memoryBitmap = Win32.Gdi32.CreateCompatibleBitmap(_windowDC, _windowSize.Width, _windowSize.Height);
			Win32.Gdi32.SelectObject(_memoryDC, _memoryBitmap);
			if (Win32.GdiPlus.GdipCreateFromHDC(_memoryDC, out _graphics) == Win32.GpStatus.Ok && Win32.GdiPlus.GdipDrawImageRectI(_graphics, _splashImage, 0, 0, _windowSize.Width, _windowSize.Height) == Win32.GpStatus.Ok)
			{
				Win32.Point pptSrc = new Win32.Point
				{
					X = 0,
					Y = 0
				};
				Win32.Point pptDst = new Win32.Point
				{
					X = _windowRectangle.Left,
					Y = _windowRectangle.Top
				};
				Win32.BlendFunction pblend = new Win32.BlendFunction
				{
					AlphaFormat = Win32.AlphaFormat.AC_SRC_ALPHA,
					BlendFlags = 0,
					BlendOp = 0,
					SourceConstantAlpha = byte.MaxValue
				};
				if (!Win32.User32.UpdateLayeredWindow(_window, _screenDC, ref pptDst, ref _windowSize, _memoryDC, ref pptSrc, 0u, ref pblend, 2u))
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
			}
		}

		private void ReleaseAllResource()
		{
			IntPtr splashImage = _splashImage;
			Win32.GdiPlus.GdipDisposeImage(_splashImage);
			UIntPtr gdipToken = _gdipToken;
			Win32.GdiPlus.GdiplusShutdown(_gdipToken);
			Win32.Gdi32.DeleteObject(_memoryBitmap);
			Win32.Gdi32.DeleteDC(_memoryDC);
			Win32.User32.ReleaseDC(_window, _windowDC);
			Win32.User32.ReleaseDC(IntPtr.Zero, _screenDC);
			Win32.User32.KillTimer(_window, _timer);
		}

		private IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			try
			{
				switch ((Win32.WM)msg)
				{
				case Win32.WM.DESTROY:
					Win32.User32.PostQuitMessage(0);
					return IntPtr.Zero;
				case Win32.WM.DPICHANGED:
					SetPositionAndSize(wParam.ToInt32() >> 16);
					DrawImage();
					return IntPtr.Zero;
				case Win32.WM.NCHITTEST:
				{
					IntPtr intPtr = Win32.User32.DefWindowProc(hWnd, msg, wParam, lParam);
					return (intPtr == (IntPtr)2) ? ((IntPtr)0) : intPtr;
				}
				default:
					return Win32.User32.DefWindowProc(hWnd, msg, wParam, lParam);
				}
			}
			catch
			{
				Win32.User32.DestroyWindow(_window);
				return IntPtr.Zero;
			}
		}
	}
}
