using System.Windows;
using System.Windows.Input;

namespace PPTService
{
	public static class DeviceInput
	{
		public static void CaptureDevice(this UIElement element, TouchDevice device = null)
		{
			element.CaptureMouse();
			element.CaptureStylus();
			if (device != null)
			{
				element.CaptureTouch(device);
			}
		}

		public static void ReleaseDeviceCapture(this UIElement element, TouchDevice device = null)
		{
			element.ReleaseMouseCapture();
			element.ReleaseStylusCapture();
			if (device != null)
			{
				element.ReleaseTouchCapture(device);
			}
		}
	}
}
