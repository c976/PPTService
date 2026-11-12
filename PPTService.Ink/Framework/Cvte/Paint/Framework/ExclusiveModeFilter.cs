using System.Windows;

namespace Cvte.Paint.Framework
{
	public class ExclusiveModeFilter : ActiveModeFilter
	{
		private class ExclusiveMode : Mode
		{
			internal static ExclusiveMode Instance { get; } = new ExclusiveMode();
		}

		public static readonly DependencyProperty IsExclusiveModeProperty = DependencyProperty.RegisterAttached("IsExclusiveMode", typeof(bool), typeof(ExclusiveModeFilter), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

		private bool _isElementExclusive;

		public ExclusiveModeFilter()
		{
			SetPriority(10);
			OnCanBeActivated = (ActiveModeFilter filter) => ((ExclusiveModeFilter)filter)._isElementExclusive;
		}

		protected override void OnInputStarting(ModeInputEventArgs e)
		{
			if (ActiveModeController.ActiveMode != null)
			{
				return;
			}
			Mode currentMode = CurrentMode;
			if (currentMode == null || ((IElementInteractionEnabled)currentMode).IsElementInteractionEnabled)
			{
				DependencyObject hittedSource = e.HittedSource;
				if (hittedSource != null && GetIsExclusiveMode(hittedSource))
				{
					_isElementExclusive = true;
					ActivateTargetMode();
				}
			}
		}

		protected override void OnDeviceDown(ModeInputEventArgs e)
		{
			if (CanBeActivated())
			{
				ActivateTargetMode();
			}
		}

		protected override void OnDeviceMove(ModeInputEventArgs e)
		{
			if (CanBeActivated())
			{
				ActivateTargetMode();
			}
		}

		protected override void ActivateTargetMode()
		{
			if (IsEnabled)
			{
				base.ActivateTargetMode();
			}
		}

		protected override void OnWholeProgressCompleted()
		{
			_isElementExclusive = false;
			if (ActiveModeController.ActiveMode == ExclusiveMode.Instance)
			{
				DeactivateTargetMode();
			}
		}

		public static void SetIsExclusiveMode(DependencyObject element, bool value)
		{
			element.SetValue(IsExclusiveModeProperty, value);
		}

		public static bool GetIsExclusiveMode(DependencyObject element)
		{
			return (bool)element.GetValue(IsExclusiveModeProperty);
		}

		protected override Mode GetTargetMode()
		{
			return ExclusiveMode.Instance;
		}
	}
}
