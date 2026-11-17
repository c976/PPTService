using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Settings;
using PPTService.Paint.Inks;
using PPTService.Ink.Hardware.Core;
using PPTService.Paint.Settings;
using PPTService.Ink.Core.Windows.Input;
using PPTService.Ink.Core.Windows.Threading;

namespace PPTService.Paint
{
	public class SmartPenPressureFilter : ActiveModeFilter, IAttachedSetting<SmartPenSettings>
	{
		private readonly DispatcherTask _resetTask;

		private const int ResetDelayInMilliseconds = 250;

		private Element _element;

		public override bool IsEnabled
		{
			get
			{
				return Settings.IsEnabled;
			}
			set
			{
				Settings.IsEnabled = value;
			}
		}

		public SmartPenSettings Settings => GetSetting<SmartPenSettings>();

		public SmartPenPressureFilter()
		{
			OnCanBeActivated = (ActiveModeFilter filter) => CurrentMode is SelectionMode || CurrentMode is EraserMode;
			_resetTask = new DispatcherTask(250.0, DeactivateTargetMode);
		}

		protected override void OnSwitchOn()
		{
			Board board = GetBoard();
			HardwareServices.SmartPen.Enable(() => (HwndSource)PresentationSource.FromVisual(board));
			OnCanBeActivated = ActiveModeFilter.ModetoFilterCanBeActivated(() => Settings.OnCanWrite);
			base.OnSwitchOn();
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (HardwareServices.SmartPen.IsEmrEnabled && !HardwareServices.SmartPen.IsEmrPenConnected && HardwareServices.SmartPen.IsPen(e.OriginEventArgs))
			{
				HardwareServices.SmartPen.OnEmrPenConnected();
			}
			if (e.Type != DeviceType.Mouse && CanBeActivated() && IsPenPressed(e))
			{
				_element = e.Element;
				ActivateTargetMode();
			}
			base.OnDeviceDown(e);
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (e.Type != DeviceType.Mouse && CanBeActivated() && IsPenPressed(e))
			{
				_element = e.Element;
				ActivateTargetMode();
			}
			else if (IsActive && IsPenPressed(e))
			{
				_resetTask.Stop();
			}
			base.OnDeviceMove(e);
		}

		protected override void OnWholeProgressCompleted()
		{
			if (IsActive)
			{
				_element?.ClearValue(DeviceInputSource.CanRespondProperty);
				_resetTask.Restart();
			}
		}

		protected override void ActivateTargetMode()
		{
			_resetTask.Stop();
			_element?.SetValue(DeviceInputSource.CanRespondProperty, false);
			base.ActivateTargetMode();
			CurrentSlide?.Unselect();
		}

		protected override void DeactivateTargetMode()
		{
			if (IsActive)
			{
				_element?.ClearValue(DeviceInputSource.CanRespondProperty);
			}
			base.DeactivateTargetMode();
		}

		protected override Mode GetTargetMode()
		{
			return new SoftPenMode
			{
				DelayInserting = false,
				MinDrawingTimeInMilliseconds = 0.0
			};
		}

		private bool IsPenPressed(DeviceInputEventArgs e)
		{
			if (IsEmrPenTipPressed(e) || IsNormalPenTipPressed())
			{
				return true;
			}
			return false;
		}

		private bool IsEmrSeletionPressed(DeviceInputEventArgs e)
		{
			StylusEventArgs e2 = e.OriginEventArgs as StylusEventArgs;
			if (e2 != null)
			{
				return HardwareServices.SmartPen.IsSelectedButtonPressed(e2);
			}
			return false;
		}

		private bool IsEmrPenTipPressed(DeviceInputEventArgs e)
		{
			if (HardwareServices.SmartPen.IsEmrPenWritable && HardwareServices.SmartPen.IsPen(e.OriginEventArgs))
			{
				return !IsEmrSeletionPressed(e);
			}
			return false;
		}

		private bool IsNormalPenTipPressed()
		{
			return HardwareServices.SmartPen.Pressure.HasValue;
		}
	}
}
