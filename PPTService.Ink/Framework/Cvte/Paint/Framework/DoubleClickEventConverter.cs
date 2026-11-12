using System;
using System.Windows;
using Cvte.Paint.Framework.Settings;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public class DoubleClickEventConverter : EventConverter
	{
		private Lazy<DeviceInputDoubleDifferentialValue> HitTestTolerance { get; set; }

		private ModeInputEventArgs FirstInput { get; set; }

		private ModeInputEventArgs PreInput { get; set; }

		private bool IsDoubleClick { get; set; }

		private DateTime PreInputTime { get; set; }

		private int ClickCount { get; set; }

		private TimeSpan MaxDoubleClickTime { get; } = TimeSpan.FromMilliseconds(500.0);

		public event EventHandler<ModeInputEventArgs> DoubleClicked;

		public DoubleClickEventConverter(InputProcessor inputProcessor)
			: base(inputProcessor)
		{
		}

		private bool CheckDistance(Point a, Point b, DeviceType type)
		{
			if (!(Math.Abs(a.X - b.X) > GetTolerance(type)))
			{
				return !(Math.Abs(a.Y - b.Y) > GetTolerance(type));
			}
			return false;
		}

		private bool CheckTime()
		{
			if (PreInputTime == default(DateTime))
			{
				return false;
			}
			return DateTime.Now - PreInputTime < MaxDoubleClickTime;
		}

		private double GetTolerance(DeviceType type)
		{
			switch (type)
			{
			case DeviceType.Mouse:
				return HitTestTolerance.Value.Mouse;
			case DeviceType.Stylus:
				return HitTestTolerance.Value.Stylus;
			case DeviceType.Touch:
				return HitTestTolerance.Value.Touch;
			default:
				throw new ArgumentOutOfRangeException("type", type, null);
			}
		}

		private void Clean()
		{
			IsDoubleClick = false;
			PreInput = null;
			FirstInput = null;
			ClickCount = 0;
			PreInputTime = default(DateTime);
		}

		private void OnDoubleClicked(ModeInputEventArgs e)
		{
			DoubleClicked?.Invoke(this, e);
		}

		protected override void OnSwitchOn()
		{
			if (HitTestTolerance == null || HitTestTolerance.IsValueCreated)
			{
				HitTestTolerance = new Lazy<DeviceInputDoubleDifferentialValue>(() => CurrentSlide.GetSettings<ThresholdBoardSettings>().HitTestTolerance);
			}
			Clean();
		}

		protected override void OnSwitchOff()
		{
			Clean();
		}

		protected override void OnInputStarting(ModeInputEventArgs e)
		{
			if (PreInput != null && (!CheckTime() || !CheckDistance(e.Position, PreInput.Position, e.Type)))
			{
				Clean();
			}
			FirstInput = e;
			IsDoubleClick = true;
			if (PreInput == null)
			{
				PreInput = e;
				PreInputTime = DateTime.Now;
			}
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (e.VirtualType == VirtualDeviceType.LostCapture)
			{
				Clean();
				return;
			}
			if (FirstInput == null)
			{
				Clean();
				return;
			}
			IsDoubleClick = e.Id == FirstInput.Id;
			if (!IsDoubleClick)
			{
				Clean();
			}
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (e.VirtualType == VirtualDeviceType.LostCapture)
			{
				Clean();
			}
			else if (IsDoubleClick)
			{
				if (FirstInput == null)
				{
					Clean();
				}
				else if (!CheckDistance(e.Position, FirstInput.Position, e.Type))
				{
					IsDoubleClick = false;
				}
			}
		}

		protected override void OnDeviceUp(ModeInputEventArgs e)
		{
			if (e.VirtualType == VirtualDeviceType.LostCapture)
			{
				Clean();
			}
			if (FirstInput == null)
			{
				Clean();
			}
		}

		protected override void OnInputCompleted(ModeInputEventArgs e)
		{
			if (!IsDoubleClick)
			{
				Clean();
				return;
			}
			if (!CheckTime())
			{
				Clean();
				return;
			}
			ClickCount++;
			if (ClickCount == 2)
			{
				OnDoubleClicked(e);
				Clean();
			}
		}
	}
}
