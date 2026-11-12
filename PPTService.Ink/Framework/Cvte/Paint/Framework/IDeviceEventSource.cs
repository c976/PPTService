using System;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public interface IDeviceEventSource
	{
		event EventHandler<DeviceEventArgs> DeviceDown;

		event EventHandler<DeviceEventArgs> DeviceMove;

		event EventHandler<DeviceEventArgs> DeviceUp;

		void RegisterEvents(IInputElement inputElement);

		void UnregisterEvents();
	}
}
