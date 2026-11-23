using System;
using PPTServiceUltra.Paint.Framework;
using PPTServiceUltra.Paint.Framework.Settings;
using PPTServiceUltra.Paint.Inks;
using PPTServiceUltra.Paint.Settings;
using PPTServiceUltra.Ink.Core.Windows.Input;

namespace PPTServiceUltra.Paint
{
	public class EraserGestureFilter : ActiveModeFilter, IAttachedSetting<EraserGestureSettings>
	{
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

		public EraserGestureSettings Settings => GetSetting<EraserGestureSettings>();

		public EraserGestureFilter()
		{
			SetPriority(100);
			EnablingDurationFromInputStarting = TimeSpan.FromMilliseconds(200.0);
		}

		protected override void OnSwitchOn()
		{
			EnablingDurationFromInputStarting = Settings.CloseDurationFromInputStarting;
			OnCanBeActivated = ActiveModeFilter.ModetoFilterCanBeActivated(() => Settings.OnCanErase);
			base.OnSwitchOn();
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (CanBeActivated() && EraserGestureHelper.IsEraserGesture(e, CurrentSlide))
			{
				ActivateTargetMode();
			}
			base.OnDeviceDown(e);
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (CanBeActivated() && EraserGestureHelper.IsEraserGesture(e, CurrentSlide))
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
			EraserMode eraserMode = ActiveModeController.ActiveMode as EraserMode;
			if (eraserMode != null)
			{
				eraserMode.ErasingCompleted -= ErasingStratergyOnErasingCompleted;
				eraserMode.ErasingCompleted += ErasingStratergyOnErasingCompleted;
			}
		}

		private void ErasingStratergyOnErasingCompleted(object sender, EventArgs eventArgs)
		{
			CurrentSlide?.ClearValue(DeviceInputSource.CanRespondProperty);
			DeactivateTargetMode();
		}

		protected override Mode GetTargetMode()
		{
			Func<EraserMode> func = GetBoard()?.GetSettings<EraserGestureSettings>()?.OnCreateEraserMode;
			if (func != null)
			{
				EraserMode eraserMode = func();
				if (eraserMode == null)
				{
					throw new InvalidOperationException("不能返回空的 ActiveMode。");
				}
				return eraserMode;
			}
			return new EraserMode();
		}
	}
}
