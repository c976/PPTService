using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using Cvte.Cache;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Inks.Hardware.RawTouchEngine;
using Cvte.Paint.Settings;
using Cvte.Reflection;
using Cvte.Windows.Input;

namespace Cvte.Paint.Inks
{
	public static class EraserGestureHelper
	{
		private static bool _useRawInputMethod = true;

		private static bool _canGetTouchDevice = true;

		private static readonly CachePool<ValueTuple<Type, string>, Func<object, object>> Cache = new CachePool<ValueTuple<Type, string>, Func<object, object>>((ValueTuple<Type, string> info) =>
		{
			ValueTuple<Type, string> valueTuple = info;
			Type item = valueTuple.Item1;
			string item2 = valueTuple.Item2;
			PropertyInfo property = item.GetProperty(item2, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				throw new MissingFieldException(item2);
			}
			return (object o) => property.GetValue(o, null);
		});

		[Obsolete("此方法命名不对，请使用 IsEraserGesture 方法")]
		public static bool IsEraserGestrue(DeviceInputEventArgs e, Slide slide)
		{
			return IsEraserGesture(e, slide);
		}

		public static bool IsEraserGesture(DeviceInputEventArgs e, Slide slide)
		{
			if (e.Type == DeviceType.Mouse)
			{
				return false;
			}
			if (slide == null)
			{
				return false;
			}
			if (!(e.OriginEventArgs is StylusEventArgs))
			{
				return false;
			}
			ValueTuple<TouchArea, EraserGestureTouchInfoSource> touchArea = GetTouchArea(e, slide);
			TouchArea item = touchArea.Item1;
			EraserGestureTouchInfoSource item2 = touchArea.Item2;
			if (item == null)
			{
				return false;
			}
			EraserGestureSettings eraserGestureSettings = GetEraserGestureSettings(slide);
			if (eraserGestureSettings == null || eraserGestureSettings.OnIsEraserGestureByInputArgs == null)
			{
				return eraserGestureSettings?.IsEraserGesture(item.Size, item2) ?? false;
			}
			return eraserGestureSettings.OnIsEraserGestureByInputArgs(e, slide);
		}

		public static Size? GetEraserSize(DeviceInputEventArgs args, Slide slide)
		{
			ValueTuple<TouchArea, EraserGestureTouchInfoSource> touchArea = GetTouchArea(args, slide);
			TouchArea item = touchArea.Item1;
			EraserGestureTouchInfoSource item2 = touchArea.Item2;
			return NormalizeSize(item?.Size, slide, item2);
		}

		private static ValueTuple<TouchArea, EraserGestureTouchInfoSource> GetTouchArea(DeviceInputEventArgs args, Slide slide)
		{
			EraserGestureSettings eraserGestureSettings = GetEraserGestureSettings(slide);
			if (eraserGestureSettings == null || eraserGestureSettings.EraserGestureTouchInfoSource == EraserGestureTouchInfoSource.None)
			{
				return new ValueTuple<TouchArea, EraserGestureTouchInfoSource>(null, EraserGestureTouchInfoSource.None);
			}
			if (eraserGestureSettings.EraserGestureTouchInfoSource == EraserGestureTouchInfoSource.TouchSdkFirst)
			{
				if (eraserGestureSettings.CanUseCVTouch && CVTouchInfoProvider.HasInitialized && CVTouchInfoProvider.IsCVTouchFrame == true)
				{
					return new ValueTuple<TouchArea, EraserGestureTouchInfoSource>(GetTouchAreaByTouchSdk(args, slide), EraserGestureTouchInfoSource.TouchSdkFirst);
				}
				TouchArea touchAreaByWpfTouchPoint = GetTouchAreaByWpfTouchPoint(args, slide);
				if (touchAreaByWpfTouchPoint != null)
				{
					return new ValueTuple<TouchArea, EraserGestureTouchInfoSource>(touchAreaByWpfTouchPoint, EraserGestureTouchInfoSource.WpfTouchPointFirst);
				}
				return new ValueTuple<TouchArea, EraserGestureTouchInfoSource>(null, EraserGestureTouchInfoSource.TouchSdkFirst);
			}
			if (eraserGestureSettings.EraserGestureTouchInfoSource == EraserGestureTouchInfoSource.WpfTouchPointFirst)
			{
				TouchArea touchAreaByWpfTouchPoint2 = GetTouchAreaByWpfTouchPoint(args, slide);
				if (touchAreaByWpfTouchPoint2 != null)
				{
					return new ValueTuple<TouchArea, EraserGestureTouchInfoSource>(touchAreaByWpfTouchPoint2, EraserGestureTouchInfoSource.WpfTouchPointFirst);
				}
				if (!eraserGestureSettings.CanUseCVTouch || !CVTouchInfoProvider.HasInitialized || CVTouchInfoProvider.IsCVTouchFrame != true)
				{
					return new ValueTuple<TouchArea, EraserGestureTouchInfoSource>(null, EraserGestureTouchInfoSource.WpfTouchPointFirst);
				}
				return new ValueTuple<TouchArea, EraserGestureTouchInfoSource>(GetTouchAreaByTouchSdk(args, slide), EraserGestureTouchInfoSource.TouchSdkFirst);
			}
			throw new NotSupportedException($"{eraserGestureSettings.EraserGestureTouchInfoSource} is not supported.");
		}

		private static TouchArea GetTouchAreaByTouchSdk(DeviceInputEventArgs args, Slide slide)
		{
			return RawTouch.GetTouchArea(slide.PointToScreen(args.Position));
		}

		private static TouchArea GetTouchAreaByWpfTouchPoint(DeviceInputEventArgs args, Slide slide)
		{
			if (!_useRawInputMethod)
			{
				return GetTouchAreaByWpfWithDevice(args, slide);
			}
			return GetTouchAreaByWpfWithRawInput(args);
		}

		private static TouchArea GetTouchAreaByWpfWithDevice(DeviceInputEventArgs args, Slide slide)
		{
			Window window = Window.GetWindow(slide);
			TouchPoint touchPoint = GetTouchDevice(args.OriginEventArgs)?.GetTouchPoint(window);
			if (touchPoint == null)
			{
				return null;
			}
			if (touchPoint.Size == Size.Empty || touchPoint.Size == default(Size))
			{
				return null;
			}
			return new TouchArea
			{
				Position = touchPoint.Position,
				Size = touchPoint.Size
			};
		}

		private static TouchArea GetTouchAreaByWpfWithRawInput(DeviceInputEventArgs args)
		{
			if (args.Type == DeviceType.Mouse)
			{
				return null;
			}
			try
			{
				RawStylusInput rawStylusInput = (RawStylusInput)GetPropertyCache(GetPropertyCache(args.OriginEventArgs, "InputReport"), "RawStylusInput");
				if (rawStylusInput == null)
				{
					return null;
				}
				StylusPoint stylusPoint = rawStylusInput.GetStylusPoints().LastOrDefault();
				double stylusPointWidthOrHeight = TouchAreaHelper.GetStylusPointWidthOrHeight(stylusPoint, true);
				double stylusPointWidthOrHeight2 = TouchAreaHelper.GetStylusPointWidthOrHeight(stylusPoint, false);
				return new TouchArea
				{
					Position = stylusPoint.ToPoint(),
					Size = new Size(stylusPointWidthOrHeight, stylusPointWidthOrHeight2)
				};
			}
			catch (MissingMethodException exception)
			{
				_useRawInputMethod = false;
				InkTracer.LogTracer.Error(exception);
				return null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static Size? NormalizeSize(Size? size, Slide slide, EraserGestureTouchInfoSource source)
		{
			if (!size.HasValue || slide == null)
			{
				return null;
			}
			Size? size2 = GetEraserSettings(slide)?.GetEraserSize();
			EraserGestureSettings eraserGestureSettings = GetEraserGestureSettings(slide);
			if (size2.HasValue && eraserGestureSettings != null)
			{
				double eraserSizeRatio = eraserGestureSettings.GetEraserSizeRatio(size.Value, source);
				Size value = new Size(size2.Value.Width * eraserSizeRatio, size2.Value.Height * eraserSizeRatio);
				Size? maxEraserSize = eraserGestureSettings.MaxEraserSize;
				if (maxEraserSize.HasValue)
				{
					value.Width = Math.Min(value.Width, maxEraserSize.Value.Width);
					value.Height = Math.Min(value.Height, maxEraserSize.Value.Height);
				}
				return value;
			}
			return null;
		}

		private static TouchDevice GetTouchDevice(InputEventArgs args)
		{
			TouchEventArgs e = args as TouchEventArgs;
			if (e != null)
			{
				return e.TouchDevice;
			}
			StylusEventArgs e2 = args as StylusEventArgs;
			if (e2 == null)
			{
				return null;
			}
			try
			{
				if (!_canGetTouchDevice)
				{
					return null;
				}
				return (TouchDevice)Reflector.GetProperty(e2.StylusDevice, "TouchDevice");
			}
			catch
			{
				_canGetTouchDevice = false;
				return null;
			}
		}

		[CanBeNull]
		private static EraserGestureSettings GetEraserGestureSettings(Slide slide)
		{
			return slide?.GetSettings<EraserGestureSettings>();
		}

		[CanBeNull]
		private static EraserSettings GetEraserSettings(Slide slide)
		{
			return slide?.GetSettings<EraserSettings>();
		}

		private static object GetPropertyCache(object obj, string propertyName)
		{
			return Cache[new ValueTuple<Type, string>(obj.GetType(), propertyName)](obj);
		}
	}
}
