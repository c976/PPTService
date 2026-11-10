using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace Cvte.Windows
{
	public sealed class MonitorDevice
	{
		private readonly Screen _screen;

		private readonly Lazy<string> _name;

		private readonly Lazy<Dpi> _dpi;

		internal Screen Win32Screen => _screen;

		public bool IsPrimary => Equals(_screen.Primary);

		public string Id { get; private set; }

		public string Name => _name.Value;

		public Dpi Dpi => _dpi.Value;

		public Rect PixelBounds
		{
			get
			{
				Rectangle bounds = _screen.Bounds;
				return new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
			}
		}

		public System.Windows.Size Size
		{
			get
			{
				Rectangle bounds = _screen.Bounds;
				return new System.Windows.Size((double)bounds.Width / Dpi.FactorX, (double)bounds.Height / Dpi.FactorY);
			}
		}

		public Rect PixelWorkingArea
		{
			get
			{
				Rectangle workingArea = _screen.WorkingArea;
				return new Rect(workingArea.X, workingArea.Y, workingArea.Width, workingArea.Height);
			}
		}

		internal MonitorDevice(Screen screen)
		{
			Func<Dpi> func = null;
			_screen = screen;
			Id = screen.DeviceName;
			_name = new Lazy<string>(screen.GetDeviceFriendlyName, LazyThreadSafetyMode.ExecutionAndPublication);
			func = () =>
			{
				FieldInfo field = typeof(Screen).GetField("hmonitor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
				IntPtr hmonitor = (IntPtr)field.GetValue(_screen);
				return DpiExtensions.GetMonitorDpi(hmonitor);
			};
			_dpi = new Lazy<Dpi>(func, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public bool Equals(MonitorDevice other)
		{
			return object.Equals(Win32Screen, other.Win32Screen);
		}

		public override bool Equals(object obj)
		{
			MonitorDevice monitorDevice = obj as MonitorDevice;
			if (monitorDevice == null)
			{
				return false;
			}
			return Equals(monitorDevice);
		}

		public override int GetHashCode()
		{
			return _screen.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Name} ({PixelBounds})";
		}
	}
}
