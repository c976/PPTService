using System;
using System.ComponentModel;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Controls;

namespace Cvte.Paint.Framework
{
	public abstract class ClickModeFilter : Filter
	{
		private class NothingMode : Mode
		{
			internal static NothingMode Instance { get; } = new NothingMode();
		}

		private readonly Mode _mode = NothingMode.Instance;

		private bool _isCurrentModeEnabled = true;

		[DefaultValue(true)]
		protected bool IsCurrentModeEnabled
		{
			get
			{
				return _isCurrentModeEnabled;
			}
			set
			{
				if (_isCurrentModeEnabled != value)
				{
					_isCurrentModeEnabled = value;
					if (value)
					{
						ActiveModeController.InvalidateActiveMode();
					}
					else
					{
						ActiveModeController.SetActiveMode(_mode);
					}
				}
			}
		}

		protected bool? IsElementInteractionEnabled => ElementHitTester.IsHitTestVisible(CurrentSlide);

		[Pure]
		[CanBeNull]
		protected static InteractiveButton HitInteractiveButton([NotNull] Slide slide, Point hitPoint, ElementHitTestLevel level = ElementHitTestLevel.Normal)
		{
			return ElementHitTester.HitInteractiveButton(slide, hitPoint, level);
		}

		protected void PerformClick([NotNull] InteractiveButton button)
		{
			if (button == null)
			{
				throw new ArgumentNullException("button");
			}
			button.PerformClick();
		}
	}
}
