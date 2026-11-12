using System;
using System.Windows;
using Cvte.ComponentModel;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Settings;
using Cvte.Windows;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public sealed class ModeHitTestProvider : IModeHitTestProvider
	{
		private readonly Lazy<Slide> _slideLazy;

		private readonly Lazy<Element> _elementLazy;

		public Slide Slide => _slideLazy.Value;

		public Element Element => _elementLazy.Value;

		public ModeHitTestProvider([CanBeNull] DependencyObject hittedSource, Point position, DeviceType type)
		{
			ModeHitTestProvider modeHitTestProvider = this;
			_slideLazy = new Lazy<Slide>(() => (hittedSource != null) ? modeHitTestProvider.FindHittedSlide(hittedSource) : null, false);
			_elementLazy = new Lazy<Element>(() => modeHitTestProvider.Slide?.HitElement(position, modeHitTestProvider.GetAllowance(type)), false);
		}

		private Slide FindHittedSlide([NotNull] DependencyObject hittedSource)
		{
			if (hittedSource == null)
			{
				throw new ArgumentNullException("hittedSource");
			}
			Slide slide = Slide.GetSlide(hittedSource);
			if (slide == null)
			{
				Board board = hittedSource.VisualAncestor<Board>();
				if (board == null && DebuggingProperties.IsDebug)
				{
					throw new ArgumentException("无法将命中测试传入的源定位到有效的 Board 中。");
				}
				slide = board?.CurrentSlide;
			}
			return slide;
		}

		private double GetAllowance(DeviceType type)
		{
			DeviceInputDoubleDifferentialValue deviceInputDoubleDifferentialValue = Slide?.GetSettings<ThresholdBoardSettings>()?.HitTestTolerance;
			if (deviceInputDoubleDifferentialValue == null)
			{
				return 1.0;
			}
			switch (type)
			{
			case DeviceType.Mouse:
				return deviceInputDoubleDifferentialValue.Mouse;
			case DeviceType.Touch:
				return deviceInputDoubleDifferentialValue.Touch;
			case DeviceType.Stylus:
				return deviceInputDoubleDifferentialValue.Stylus;
			default:
				return 1.0;
			}
		}
	}
}
