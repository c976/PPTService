using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Cvte.IO;
using Cvte.Windows;

namespace Cvte.Paint.Inks.Hardware.RawTouchEngine
{
	public static class CVTouchInfoProvider
	{
		private static readonly TouchSDK.DeviceInfo[] DeviceInfo;

		private static readonly TouchSDK.FuncTouchPointData MultiTouhCallbackDelegate;

		private static readonly TouchSDK.FuncHotplugDevInfo HotplugCallbackDelegate;

		private static readonly double ScreenWidth;

		private static readonly double ScreenHeight;

		public static bool HasInitialized { get; private set; }

		public static bool CanUseCVTouch { get; set; }

		public static bool? IsCVTouchFrame { get; private set; }

		public static event EventHandler<RawTouchEventArgs> RawTouchDown;

		public static event EventHandler<RawTouchEventArgs> RawTouchMove;

		public static event EventHandler<RawTouchEventArgs> RawTouchUp;

		static CVTouchInfoProvider()
		{
			DeviceInfo = new TouchSDK.DeviceInfo[8];
			ScreenWidth = SystemParameters.PrimaryScreenWidth;
			ScreenHeight = SystemParameters.PrimaryScreenHeight;
			MultiTouhCallbackDelegate = MultiTouchCallback;
			HotplugCallbackDelegate = HotplugCallback;
		}

		public static void Initialize()
		{
			if (CanUseCVTouch)
			{
				RawTouch.Initialize();
				new Task(InitializeWorker).Start();
			}
		}

		// ILSpy 未能还原原程序集里 Initialize 的本地函数体（希沃 RawTouch 硬件轮询）。
		// 移植到 PPT 小工具时不走希沃硬件通道，这里保留空的占位实现以保证源码可编译。
		private static void InitializeWorker()
		{
		}

		private static void HotplugCallback(TouchSDK.DeviceInfo devInfo, byte attached, IntPtr callbackobject)
		{
			bool flag = attached == 1;
			Log.Info($"HotplugCallback: plug in = {flag}");
			if (!flag)
			{
				return;
			}
			new Timer((object obj) =>
			{
				try
				{
					TouchSDK.EnableRawData(devInfo);
					TouchSDK.EnableTouchWidthData(devInfo);
				}
				catch (Exception e)
				{
					Log.Error(e);
				}
			}, null, 0, 1500);
		}

		private static void MultiTouchCallback(TouchSDK.DeviceInfo devInfo, IntPtr pdata, int maxpointnum, int nValidPointNum, IntPtr pCbObj)
		{
			if (Application.Current == null || Application.Current.Dispatcher == null)
			{
				return;
			}
			for (int i = 0; i < maxpointnum; i++)
			{
				TouchSDK.TouchPointData touchPointData = (TouchSDK.TouchPointData)Marshal.PtrToStructure((IntPtr)(pdata.ToInt64() + i * Marshal.SizeOf(typeof(TouchSDK.TouchPointData))), typeof(TouchSDK.TouchPointData));
				RawTouchEventArgs args = new RawTouchEventArgs
				{
					Id = touchPointData.c,
					Position = new Point((double)touchPointData.x * ScreenWidth / 32767.0 * Dpi.System.FactorX, (double)touchPointData.y * ScreenHeight / 32767.0 * Dpi.System.FactorY),
					Width = touchPointData.w,
					Height = touchPointData.h
				};
				switch (touchPointData.ts)
				{
				case TouchSDK.TouchState.TOUCH_DOWN:
					Application.Current.Dispatcher.BeginInvoke((Action)(() =>
					{
						OnRawTouchDown(args);
					}));
					TouchDebugger.Log("TOUCH_DOWN\r\n" + args);
					break;
				case TouchSDK.TouchState.TOUCH_MOVE:
					Application.Current.Dispatcher.BeginInvoke((Action)(() =>
					{
						OnRawTouchMove(args);
					}));
					TouchDebugger.Log("TOUCH_MOVE\r\n" + args);
					break;
				case TouchSDK.TouchState.TOUCH_UP:
					Application.Current.Dispatcher.BeginInvoke((Action)(() =>
					{
						OnRawTouchUp(args);
					}));
					TouchDebugger.Log("TOUCH_UP\r\n" + args);
					break;
				}
			}
		}

		private static void Unload()
		{
			try
			{
				Log.Info("TouchSDK unloading...");
				int touchDeviceCount = TouchSDK.GetTouchDeviceCount();
				Log.Info($"TouchSDK device count = {touchDeviceCount}");
				for (int i = 0; i < touchDeviceCount; i++)
				{
					try
					{
						TouchSDK.EnableTouch(DeviceInfo[0]);
						TouchSDK.ExitTouch();
					}
					catch (Exception e)
					{
						Log.Error($"TouchSDK DeviceInfo #{i} unloading failed", e);
					}
				}
				Log.Info("TouchSDK unloaded...");
			}
			catch (Exception e2)
			{
				Log.Error(e2);
			}
		}

		private static void OnRawTouchDown(RawTouchEventArgs e)
		{
			RawTouchDown?.Invoke(null, e);
		}

		private static void OnRawTouchMove(RawTouchEventArgs e)
		{
			RawTouchMove?.Invoke(null, e);
		}

		private static void OnRawTouchUp(RawTouchEventArgs e)
		{
			RawTouchUp?.Invoke(null, e);
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetDllDirectory(string lpPathName);

		private static void SetDllImportDir()
		{
			string location = Assembly.GetEntryAssembly().Location;
			string text = Path.Combine(path2: (IntPtr.Size == 4) ? "x86" : "x64", path1: Directory.GetParent(location).FullName);
			if (Directory.Exists(text))
			{
				SetDllDirectory(text);
			}
		}
	}
}
