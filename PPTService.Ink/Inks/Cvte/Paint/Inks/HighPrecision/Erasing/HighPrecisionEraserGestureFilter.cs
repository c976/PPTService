using System;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;
using Cvte.Windows.Input;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public class HighPrecisionEraserGestureFilter : ActiveModeFilter, IAttachedSetting<HighPrecisionEraserGestureSettings>
	{
		public HighPrecisionEraserGestureSettings Settings => GetSetting<HighPrecisionEraserGestureSettings>();

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

		public HighPrecisionEraserGestureFilter()
		{
			SetPriority(100);
			EnablingDurationFromInputStarting = TimeSpan.FromMilliseconds(200.0);
		}

		protected override void OnSwitchOn()
		{
			OnCanBeActivated = ActiveModeFilter.ModetoFilterCanBeActivated(() => Settings.OnCanErase);
			base.OnSwitchOn();
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (CanBeActivated() && Settings.IsEraserGesture(e))
			{
				ActivateTargetMode();
			}
			base.OnDeviceDown(e);
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (CanBeActivated() && Settings.IsEraserGesture(e))
			{
				ActivateTargetMode();
			}
			base.OnDeviceMove(e);
		}

		protected override void ActivateTargetMode()
		{
			CurrentSlide?.Unselect();
			base.ActivateTargetMode();
			CurrentSlide?.SetValue(DeviceInputSource.CanRespondProperty, false);
			HighPrecisionEraserMode highPrecisionEraserMode = ActiveModeController.ActiveMode as HighPrecisionEraserMode;
			if (highPrecisionEraserMode != null)
			{
				highPrecisionEraserMode.ErasingCompleted -= ErasingStratergyOnErasingCompleted;
				highPrecisionEraserMode.ErasingCompleted += ErasingStratergyOnErasingCompleted;
			}
		}

		private void ErasingStratergyOnErasingCompleted(object sender, EventArgs eventArgs)
		{
			CurrentSlide?.ClearValue(DeviceInputSource.CanRespondProperty);
			DeactivateTargetMode();
		}

		protected override Mode GetTargetMode()
		{
			Func<HighPrecisionEraserMode> eraserModeCreator = Settings.EraserModeCreator;
			if (eraserModeCreator != null)
			{
				HighPrecisionEraserMode highPrecisionEraserMode = eraserModeCreator();
				if (highPrecisionEraserMode == null)
				{
					throw new InvalidOperationException("不能返回空的 ActiveMode。");
				}
				return highPrecisionEraserMode;
			}
			return new HighPrecisionEraserMode();
		}
	}
}
