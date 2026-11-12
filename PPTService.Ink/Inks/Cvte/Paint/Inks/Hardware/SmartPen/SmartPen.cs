using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Cvte.ComponentModel;

namespace Cvte.Paint.Inks.Hardware.SmartPen
{
	public static class SmartPen
	{
		private const long PenTipDownCode = 10L;

		private const long PenTipUpCode = 26L;

		private const long SquareButtonDownCode = 1L;

		private const long SquareButtonUpCode = 17L;

		private static readonly string SmartPenConfigFile;

		private static readonly Dictionary<IntPtr, WeakReference<HwndSource>> WindowInstances;

		private static readonly HashSet<IntPtr> Windows;

		private static SmartPenType? _connnectedPen;

		public static double? Pressure { get; private set; }

		public static bool IsSquareButtonPressed { get; private set; }

		[Obsolete("在10月5日前删除这个属性")]
		internal static DateTime LastDownTime { get; set; }

		public static bool IsConnected
		{
			get
			{
				if (Connection != SmartPenType.SP05 && Connection != SmartPenType.SP05A && Connection != SmartPenType.SP06)
				{
					return Connection == SmartPenType.SP08;
				}
				return true;
			}
		}

		public static SmartPenType Connection
		{
			get
			{
				if (!_connnectedPen.HasValue)
				{
					_connnectedPen = TestIsConnected();
				}
				return _connnectedPen.Value;
			}
			private set
			{
				_connnectedPen = value;
			}
		}

		internal static bool IsEmrPenConnected { get; set; }

		public static bool IsEmrEnabled { get; set; }

		public static bool IsEmrPenWritable
		{
			get
			{
				if (IsEmrEnabled)
				{
					return IsEmrPenConnected;
				}
				return false;
			}
		}

		internal static Action OnEmrPenConnected => () =>
		{
			if (IsEmrEnabled)
			{
				IsEmrPenConnected = true;
				_connnectedPen = SmartPenType.SP08;
				OnIsConnectedChanged();
			}
		};

		public static event EventHandler IsConnectedChanged;

		static SmartPen()
		{
			WindowInstances = new Dictionary<IntPtr, WeakReference<HwndSource>>();
			Windows = new HashSet<IntPtr>();
			try
			{
				string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seewo", "SmartPen");
				SmartPenConfigFile = Path.Combine(text, "config.ini");
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(text);
				fileSystemWatcher.Changed += OnConfigFileChanged;
				fileSystemWatcher.Created += OnConfigFileChanged;
				fileSystemWatcher.Deleted += OnConfigFileChanged;
				fileSystemWatcher.EnableRaisingEvents = true;
			}
			catch (Exception exception)
			{
				InkTracer.LogTracer.Error(exception);
			}
		}

		private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
		{
			bool num = _connnectedPen == SmartPenType.NotConnected;
			Connection = TestIsConnected();
			bool flag = _connnectedPen == SmartPenType.NotConnected;
			if (num != flag)
			{
				OnIsConnectedChanged();
			}
		}

		private static SmartPenType TestIsConnected()
		{
			SmartPenType smartPenType = SmartPenType.NotConnected;
			if (IsEmrEnabled)
			{
				smartPenType = TestEmrPenConnected();
			}
			if (smartPenType == SmartPenType.NotConnected)
			{
				smartPenType = TestNormalPenConnected();
			}
			return smartPenType;
		}

		private static SmartPenType TestNormalPenConnected()
		{
			string smartPenConfigFile = SmartPenConfigFile;
			if (File.Exists(smartPenConfigFile))
			{
				try
				{
					string text = IniReadValue(smartPenConfigFile, "SmartPen", "isPenConnected").ToLower();
					string text2 = IniReadValue(smartPenConfigFile, "SmartPen", "PenVersion").ToUpper(CultureInfo.InvariantCulture);
					if (!(text != "true"))
					{
						switch (text2)
						{
						case "SP01":
							return SmartPenType.SP01;
						case "SP05":
							return SmartPenType.SP05;
						case "SP05A":
							return SmartPenType.SP05A;
						default:
							return SmartPenType.Unkown;
						}
					}
					return SmartPenType.NotConnected;
				}
				catch (Exception exception)
				{
					InkTracer.LogTracer.Error(exception);
					return SmartPenType.NotConnected;
				}
			}
			return SmartPenType.NotConnected;
		}

		public static void Enable(Func<HwndSource> hwndSourceCreator)
		{
			HwndSource hwndSource = hwndSourceCreator();
			if (hwndSource == null)
			{
				if (DebuggingProperties.IsDebug)
				{
					throw new ArgumentException("获取不到 source 的值，此异常仅在 Debug 下抛出。如遇到，请将德熙拉过来");
				}
				return;
			}
			IntPtr handle = hwndSource.Handle;
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentException("无效的窗口句柄。", "hwnd");
			}
			if (!Windows.Contains(handle))
			{
				Windows.Add(handle);
				hwndSource.AddHook(WinProc);
				WindowInstances.Add(hwndSource.Handle, new WeakReference<HwndSource>(hwndSource));
			}
		}

		private static IntPtr WinProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
		{
			WeakReference<HwndSource> value;
			if (msg == 49876 && WindowInstances.TryGetValue(hwnd, out value))
			{
				HwndSource target;
				if (value.TryGetTarget(out target))
				{
					switch (lparam.ToInt64())
					{
					case 10L:
						Pressure = 1.0;
						break;
					case 26L:
						Pressure = null;
						break;
					case 1L:
						IsSquareButtonPressed = true;
						break;
					case 17L:
						IsSquareButtonPressed = false;
						break;
					}
				}
				else
				{
					WindowInstances.Remove(hwnd);
				}
			}
			return IntPtr.Zero;
		}

		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		private static string IniReadValue(string path, string section, string key)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			GetPrivateProfileString(section, key, "", stringBuilder, 1024, path);
			return stringBuilder.ToString();
		}

		private static void OnIsConnectedChanged()
		{
			Application.Current?.Dispatcher?.InvokeAsync(() =>
			{
				IsConnectedChanged?.Invoke(null, EventArgs.Empty);
			});
		}

		private static SmartPenType TestEmrPenConnected()
		{
			if (!IsEmrPenConnected)
			{
				return SmartPenType.NotConnected;
			}
			return SmartPenType.SP08;
		}
	}
}
