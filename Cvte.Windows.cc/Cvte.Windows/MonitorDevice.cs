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
				double num = bounds.Width;
				while (true)
				{
					Dpi dpi;
					if (0 == 0)
					{
						dpi = Dpi;
						goto IL_0019;
					}
					goto IL_0021;
					IL_0020:
					double num2;
					num /= num2;
					goto IL_0021;
					IL_0021:
					if (false)
					{
						goto IL_0019;
					}
					num2 = bounds.Height;
					Dpi dpi2 = Dpi;
					while (true)
					{
						num2 /= dpi2.FactorY;
						if (1 == 0)
						{
							break;
						}
						if (0 == 0)
						{
							return new System.Windows.Size(num, num2);
						}
					}
					goto IL_0020;
					IL_0019:
					if (5 == 0)
					{
						continue;
					}
					num2 = dpi.FactorX;
					goto IL_0020;
				}
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
			_screen = screen;
			Id = screen.DeviceName;
			_name = new Lazy<string>(screen.GetDeviceFriendlyName, LazyThreadSafetyMode.ExecutionAndPublication);
			Func<Dpi> valueFactory = () =>
			{
				FieldInfo field = typeof(Screen).GetField("hmonitor", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);
				IntPtr hmonitor = (IntPtr)field.GetValue(_screen);
				return DpiExtensions.GetMonitorDpi(hmonitor);
			};
			_dpi = new Lazy<Dpi>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public bool Equals(MonitorDevice other)
		{
			return object.Equals(Win32Screen, other.Win32Screen);
		}

		public override bool Equals(object obj)
		{
			int result;
			while (true)
			{
				MonitorDevice monitorDevice;
				if (0 == 0)
				{
					monitorDevice = obj as MonitorDevice;
				}
				if (monitorDevice == null)
				{
					if (false)
					{
						continue;
					}
					if (5 != 0)
					{
						result = 0;
						goto IL_0017;
					}
				}
				result = (Equals(monitorDevice) ? 1 : 0);
				if (0 == 0)
				{
					break;
				}
				goto IL_0017;
				IL_0017:
				return (byte)result != 0;
			}
			return (byte)result != 0;
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
