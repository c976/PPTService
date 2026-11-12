using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;
using Cvte.Windows.Threading;

namespace Cvte.Paint.Inks.Hardware.SmartPen
{
	public class SmartPenSelectionButtonFilter : ActiveModeFilter, IAttachedSetting<SmartPenSelectionButtonSettings>
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

		public SmartPenSelectionButtonSettings Settings => GetSetting<SmartPenSelectionButtonSettings>();

		public SmartPenSelectionButtonFilter()
		{
			OnCanBeActivated = (ActiveModeFilter filter) => CurrentMode is WritingPenMode || CurrentMode is EraserMode;
		}

		protected override void OnSwitchOn()
		{
			OnCanBeActivated = ActiveModeFilter.ModetoFilterCanBeActivated(() => Settings.OnCanSelect);
			base.OnSwitchOn();
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (CanBeActivated() && IsSquareButtonPressed())
			{
				ActivateTargetMode();
			}
			base.OnDeviceDown(e);
		}

		protected override void OnWholeProgressCompleted()
		{
			if (IsActive)
			{
				DispatcherTask.Start(DeactivateTargetMode);
			}
		}

		protected override Mode GetTargetMode()
		{
			return new SelectionMode();
		}

		private bool IsSquareButtonPressed()
		{
			if (SmartPen.IsSquareButtonPressed)
			{
				return true;
			}
			return false;
		}
	}
}
