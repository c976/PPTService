using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Cvte.Windows.Mathematics;

namespace Cvte.Paint.Inks.Hardware
{
	public static class EmrPenManager
	{
		private static readonly Dictionary<int, InputDeviceInfo> InputDeviceInfoDictionary;

		public static InputDevice CurrentInputDevice;

		private const double BlackPressure = 2044.0;

		private const double WhitePressure = 2045.0;

		private const double PressureTollerance = 1E-05;

		private const string PenA = "PEN A";

		private const string PenB = "PEN B";

		private const string Touch = "TOUCH";

		private const string Pen = "PEN";

		public static EMRMode CurrentEMRMode { get; set; }

		static EmrPenManager()
		{
			InputDeviceInfoDictionary = new Dictionary<int, InputDeviceInfo>();
			CurrentInputDevice = InputDevice.Other;
			CurrentEMRMode = EMRMode.Pen;
			foreach (TabletDevice item in Tablet.TabletDevices.OfType<TabletDevice>())
			{
				InputDeviceInfo value = new InputDeviceInfo(item.Id, item.Name, item.ProductId, item.Type);
				InputDeviceInfoDictionary.Add(item.Id, value);
			}
		}

		public static bool IsEmrEnable()
		{
			return InputDeviceInfoDictionary.Values.Any((InputDeviceInfo device) => !string.IsNullOrEmpty(device.Name) && _003CIsEmrEnable_003Eg__IsContainsPen_007C7_1(device));
		}

		public static bool IsPen(InputEventArgs e)
		{
			InputDevice inputDevice = GetInputDevice(e);
			if (inputDevice != InputDevice.PenA)
			{
				return inputDevice == InputDevice.PenB;
			}
			return true;
		}

		public static bool IsSelectedButtonPressed(StylusEventArgs e)
		{
			StylusPointCollection stylusPoints = e.GetStylusPoints((IInputElement)e.Source);
			double num = Math.Abs(Math.Floor((double)(((stylusPoints.Count > 0) ? stylusPoints[0].PressureFactor : 0f) * 2047f) + 0.5));
			if (!(Math.Abs(num - 2044.0) < 1E-05))
			{
				return Math.Abs(num - 2045.0) < 1E-05;
			}
			return true;
		}

		public static InputDevice GetInputDevice(InputEventArgs e)
		{
			InputDevice result = InputDevice.Other;
			StylusEventArgs e2 = e as StylusEventArgs;
			if (e2 != null)
			{
				try
				{
					string name = e2.StylusDevice.TabletDevice.Name;
					StylusPointCollection stylusPoints = e2.StylusDevice.GetStylusPoints((IInputElement)e2.Source);
					float pressureFactor = ((stylusPoints.Count > 0) ? stylusPoints[0].PressureFactor : 0f);
					return GetInputDevice(name, pressureFactor);
				}
				catch
				{
				}
			}
			return result;
		}

		private static InputDevice GetInputDevice(string tabletDeviceName, float pressureFactor)
		{
			InputDevice result = InputDevice.Other;
			if (!string.IsNullOrWhiteSpace(tabletDeviceName))
			{
				try
				{
					tabletDeviceName = tabletDeviceName.ToUpper();
					switch (CurrentEMRMode)
					{
					case EMRMode.Pen:
						if (tabletDeviceName.Contains("PEN A"))
						{
							result = InputDevice.PenA;
						}
						else if (tabletDeviceName.Contains("PEN B"))
						{
							result = InputDevice.PenB;
						}
						else if (tabletDeviceName.Contains("TOUCH"))
						{
							result = InputDevice.Touch;
						}
						break;
					case EMRMode.Multitouch:
						if (tabletDeviceName.Contains("PEN"))
						{
							result = ((!Nearly.Equals(Math.Floor((double)(pressureFactor * 2047f) + 0.5) % 2.0, 1.0)) ? InputDevice.PenB : InputDevice.PenA);
						}
						else if (tabletDeviceName.Contains("TOUCH"))
						{
							result = InputDevice.Touch;
						}
						break;
					default:
						if (tabletDeviceName.Contains("PEN"))
						{
							result = InputDevice.PenA;
						}
						else if (tabletDeviceName.Contains("TOUCH"))
						{
							result = InputDevice.Touch;
						}
						break;
					}
				}
				catch
				{
				}
			}
			return result;
		}

		[CompilerGenerated]
		internal static bool _003CIsEmrEnable_003Eg__IsContainsPen_007C7_1(InputDeviceInfo device)
		{
			if (!device.Name.Contains("PEN A"))
			{
				return device.Name.Contains("PEN B");
			}
			return true;
		}
	}
}
