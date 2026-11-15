using System.Windows;

namespace PPTService.Ink.Core.Windows.Input
{
	internal static class MessageTouchDeviceHelper
	{
		public static readonly DependencyProperty WindowTouchDeviceProperty = DependencyProperty.RegisterAttached("WindowTouchDevice", typeof(MessageTouchDevice), typeof(MessageTouchDeviceHelper), new PropertyMetadata((object)null));

		public static MessageTouchDevice GetDevice(Window window, int dwId)
		{
			MessageTouchDevice messageTouchDevice = GetWindowTouchDevice(window);
			if (messageTouchDevice == null)
			{
				messageTouchDevice = new MessageTouchDevice(dwId, window);
				SetWindowTouchDevice(window, messageTouchDevice);
			}
			return messageTouchDevice;
		}

		public static void Remove(Window window)
		{
			SetWindowTouchDevice(window, null);
		}

		public static void SetWindowTouchDevice(DependencyObject element, MessageTouchDevice value)
		{
			element.SetValue(WindowTouchDeviceProperty, value);
		}

		public static MessageTouchDevice GetWindowTouchDevice(DependencyObject element)
		{
			return (MessageTouchDevice)element.GetValue(WindowTouchDeviceProperty);
		}
	}
}
