using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.Input
{
	public class SimulatedTouchByWindowsMessage
	{
		private static readonly List<IntPtr> HasOpenedTouchFixList = new List<IntPtr>();

		public static bool HasDisabledTouch { get; private set; }

		public static void DisableWpfTouch()
		{
			if (!HasDisabledTouch)
			{
				TouchTabletHelper.DisableWPFTabletSupport();
				HasDisabledTouch = true;
			}
		}

		public static void RegisterMessageTouch(Window window)
		{
			if (window == null)
			{
				throw new ArgumentNullException("window");
			}
			IntPtr hWnd = new WindowInteropHelper(window).Handle;
			if (hWnd == IntPtr.Zero)
			{
				throw new InvalidOperationException("请在SourceInitialized之后调用这个方法");
			}
			if (!HasOpenedTouchFixList.Any((IntPtr i) => i == hWnd))
			{
				HasOpenedTouchFixList.Add(hWnd);
				TouchNativeMethods.RegisterTouchWindow(hWnd, 2u);
				HwndSource.FromHwnd(hWnd)?.AddHook(delegate(IntPtr hwnd, int msg, IntPtr param, IntPtr lParam, ref bool handled)
				{
					HandleWindowProc(window, msg, param, lParam, ref handled);
					return IntPtr.Zero;
				});
			}
		}

		private static void HandleWindowProc(Window window, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg != 576)
			{
				return;
			}
			int num = wParam.ToInt32() & 0xFFFF;
			TouchNativeMethods.TOUCHINPUT[] array = new TouchNativeMethods.TOUCHINPUT[num];
			if (TouchNativeMethods.GetTouchInputInfo(lParam, num, array, TouchNativeMethods.TouchInputSize))
			{
				for (int i = 0; i < num; i++)
				{
					TouchNativeMethods.TOUCHINPUT input = array[i];
					FakeTouchInputToWindow(window, input);
				}
			}
			TouchNativeMethods.CloseTouchInputHandle(lParam);
			handled = true;
		}

		private static void FakeTouchInputToWindow(Window window, TouchNativeMethods.TOUCHINPUT input)
		{
			Point position = TransformToWpfPoint(new Point((double)input.X / 100.0, (double)input.Y / 100.0), window);
			Size size = TransformToWpfSize(new Size((double)input.CxContact / 100.0, (double)input.CyContact / 100.0), window);
			MessageTouchDevice device = MessageTouchDeviceHelper.GetDevice(window, input.DwID);
			if (!device.IsActive && input.DwFlags.HasFlag(TouchNativeMethods.TOUCHEVENTF.TOUCHEVENTF_DOWN))
			{
				device.Position = position;
				device.Size = size;
				device.Down();
			}
			else if (device.IsActive && input.DwFlags.HasFlag(TouchNativeMethods.TOUCHEVENTF.TOUCHEVENTF_MOVE))
			{
				device.Position = position;
				device.Size = size;
				device.Move();
			}
			else if (device.IsActive && input.DwFlags.HasFlag(TouchNativeMethods.TOUCHEVENTF.TOUCHEVENTF_UP))
			{
				device.Position = position;
				device.Size = size;
				device.Up();
				MessageTouchDeviceHelper.Remove(window);
			}
		}

		private static Size TransformToWpfSize(Size sizeInScreen, Window window)
		{
			double num = 1.0;
			double num2 = 1.0;
			CompositionTarget compositionTarget = PresentationSource.FromVisual(window)?.CompositionTarget;
			if (compositionTarget != null)
			{
				num = compositionTarget.TransformFromDevice.M11;
				num2 = compositionTarget.TransformFromDevice.M22;
			}
			double width = sizeInScreen.Width / num;
			double height = sizeInScreen.Height / num2;
			return new Size(width, height);
		}

		private static Point TransformToWpfPoint(Point screenPoint, Window window)
		{
			double num = 1.0;
			double num2 = 1.0;
			CompositionTarget compositionTarget = PresentationSource.FromVisual(window)?.CompositionTarget;
			if (compositionTarget != null)
			{
				num = compositionTarget.TransformFromDevice.M11;
				num2 = compositionTarget.TransformFromDevice.M22;
			}
			double x = screenPoint.X / num;
			double y = screenPoint.Y / num2;
			Point point = new Point(x, y);
			return window.PointFromScreen(point);
		}
	}
}
