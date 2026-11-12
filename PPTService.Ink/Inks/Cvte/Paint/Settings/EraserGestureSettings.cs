using System;
using System.Windows;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks;
using Cvte.Windows;
using Cvte.Windows.Input;

namespace Cvte.Paint.Settings
{
	public class EraserGestureSettings : BoardFilterSettings, IUpdateBoardSettings
	{
		private EraserGestureTouchInfoSource _eraserGestureTouchInfoSource = EraserGestureTouchInfoSource.TouchSdkFirst;

		private Size? _maxEraserSize;

		private Func<DeviceInputEventArgs, Mode, Size?> _getEraserSizeAlgorithm;

		public static Size MinEraserSize { get; set; } = EraserSettings.DefaultEraserSize;

		public static Size? DefaultMaxEraserSize { get; set; }

		public static double? GlobalEraserSizeRatio { get; set; }

		public Size? MaxEraserSize
		{
			get
			{
				return _maxEraserSize ?? DefaultMaxEraserSize;
			}
			set
			{
				if (!Nullable.Equals(value, _maxEraserSize))
				{
					_maxEraserSize = value;
					OnPropertyChanged("MaxEraserSize");
				}
			}
		}

		public TimeSpan? CloseDurationFromInputStarting { get; set; } = TimeSpan.FromMilliseconds(700.0);

		public Predicate<Mode> OnCanErase { get; set; }

		public Func<Size, EraserGestureTouchInfoSource, bool> OnIsEraserGesture { get; set; }

		public Func<DeviceInputEventArgs, Slide, bool> OnIsEraserGestureByInputArgs { get; set; }

		public bool AdjustEraserSizeByTouchArea { get; set; } = true;

		public double? EraserSizeAdjustingDurationInMilliseconds { get; set; } = 100.0;

		public bool CanUseCVTouch { get; set; }

		public EraserGestureTouchInfoSource EraserGestureTouchInfoSource
		{
			get
			{
				if (_eraserGestureTouchInfoSource == EraserGestureTouchInfoSource.TouchSdkFirst && !CanUseCVTouch)
				{
					_eraserGestureTouchInfoSource = EraserGestureTouchInfoSource.WpfTouchPointFirst;
				}
				return _eraserGestureTouchInfoSource;
			}
			set
			{
				_eraserGestureTouchInfoSource = value;
			}
		}

		public Func<Size, EraserGestureTouchInfoSource, double> TouchAreaToEraserSizeConversionAlgorithm { get; set; }

		public Func<DeviceInputEventArgs, Mode, Size?> GetEraserSizeAlgorithm
		{
			get
			{
				return _getEraserSizeAlgorithm ?? new Func<DeviceInputEventArgs, Mode, Size?>(DefaultGetEraserSizeAlgorithm);
			}
			set
			{
				_getEraserSizeAlgorithm = value;
			}
		}

		public Func<EraserMode> OnCreateEraserMode { get; set; }

		public EraserGestureSettings()
		{
			TouchAreaToEraserSizeConversionAlgorithm = DefaultTouchAreaToEraserSizeConversionAlgorithm;
		}

		internal static Size? DefaultGetEraserSizeAlgorithm(DeviceInputEventArgs args, Mode mode)
		{
			return EraserGestureHelper.GetEraserSize(args, mode.CurrentSlide);
		}

		internal bool IsEraserGesture(Size size, EraserGestureTouchInfoSource areaInfoSource)
		{
			if (OnIsEraserGesture != null)
			{
				return OnIsEraserGesture(size, areaInfoSource);
			}
			return DefaultIsEraserGesture(size, areaInfoSource);
		}

		internal double GetEraserSizeRatio(Size size, EraserGestureTouchInfoSource areaInfoSource)
		{
			if (TouchAreaToEraserSizeConversionAlgorithm != null)
			{
				return TouchAreaToEraserSizeConversionAlgorithm(size, areaInfoSource);
			}
			return DefaultTouchAreaToEraserSizeConversionAlgorithm(size, areaInfoSource);
		}

		private bool DefaultIsEraserGesture(Size touchArea, EraserGestureTouchInfoSource areaInfoSource)
		{
			if (areaInfoSource == EraserGestureTouchInfoSource.TouchSdkFirst)
			{
				if (touchArea.Width > 300.0)
				{
					return touchArea.Height > 200.0;
				}
				return false;
			}
			if (touchArea.Width > MinEraserSize.Width && touchArea.Height > MinEraserSize.Height && touchArea.Width < 3000.0)
			{
				return touchArea.Height < 3000.0;
			}
			return false;
		}

		private double DefaultTouchAreaToEraserSizeConversionAlgorithm(Size touchArea, EraserGestureTouchInfoSource areaInfoSource)
		{
			if (GlobalEraserSizeRatio.HasValue)
			{
				return GlobalEraserSizeRatio.Value;
			}
			if (areaInfoSource == EraserGestureTouchInfoSource.WpfTouchPointFirst)
			{
				double num = Math.Max(touchArea.Width / MinEraserSize.Width, touchArea.Height / MinEraserSize.Height / 1.78);
				if (Monitor.PrimaryDevice.PixelBounds.Width < 2000.0)
				{
					num *= 1.2;
				}
				return (num > 1.5) ? num : 1.5;
			}
			double length = new Vector(touchArea.Width, touchArea.Height).Length;
			double num2;
			if (length < 300.0)
			{
				num2 = 1.5;
			}
			else if (length < 720.0)
			{
				num2 = 2.2;
			}
			else
			{
				num2 = ((!(length < 1000.0)) ? 3.5 : 2.85);
			}
			if (Monitor.PrimaryDevice.PixelBounds.Width > 3000.0)
			{
				num2 *= 0.8;
			}
			return num2;
		}

		public void UpdateBoardSettings(BoardSettings boardSettings)
		{
		}
	}
}
