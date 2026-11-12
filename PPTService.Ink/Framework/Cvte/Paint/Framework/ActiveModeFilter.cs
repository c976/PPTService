using System;

namespace Cvte.Paint.Framework
{
	public abstract class ActiveModeFilter : Filter
	{
		private WeakReference<Mode> _activeMode;

		protected bool IsActive
		{
			get
			{
				if (_activeMode == null)
				{
					return false;
				}
				if (ActiveModeController.ActiveMode == null)
				{
					return false;
				}
				Mode target;
				if (_activeMode.TryGetTarget(out target))
				{
					return ActiveModeController.ActiveMode == target;
				}
				return false;
			}
		}

		public TimeSpan? EnablingDurationFromInputStarting { get; set; }

		public Predicate<ActiveModeFilter> OnCanBeActivated { get; set; }

		protected abstract Mode GetTargetMode();

		protected virtual void ActivateTargetMode()
		{
			Mode targetMode = GetTargetMode();
			Mode mode = targetMode;
			if (mode == null)
			{
				throw new InvalidOperationException("重写 GetTargetMode 方法时，不能返回空的 Mode。");
			}
			targetMode = mode;
			ActiveModeController.SetActiveMode(targetMode);
			_activeMode = new WeakReference<Mode>(targetMode);
		}

		protected virtual void DeactivateTargetMode()
		{
			ActiveModeController.InvalidateActiveMode();
			_activeMode = null;
		}

		protected bool CanBeActivated()
		{
			if (!IsActive && ActiveModeController.ActiveMode == null && GetOnCanBeActivatedDelegateResult())
			{
				return IsInEnablingDuration();
			}
			return false;
		}

		private bool IsInEnablingDuration()
		{
			if (!IsOperating)
			{
				return false;
			}
			if (!EnablingDurationFromInputStarting.HasValue)
			{
				return true;
			}
			return OperatingDuration < EnablingDurationFromInputStarting.Value;
		}

		protected bool GetOnCanBeActivatedDelegateResult()
		{
			return OnCanBeActivated?.Invoke(this) ?? true;
		}

		public static Predicate<ActiveModeFilter> ModetoFilterCanBeActivated(Func<Predicate<Mode>> delegated)
		{
			return (ActiveModeFilter filter) => delegated()?.Invoke(filter.CurrentMode) ?? true;
		}
	}
}
