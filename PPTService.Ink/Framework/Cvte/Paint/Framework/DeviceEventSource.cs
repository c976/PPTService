using System;
using System.Windows;
using System.Windows.Input;

namespace Cvte.Paint.Framework
{
	internal class DeviceEventSource : IDeviceEventSource
	{
		private IInputElement _innerEventSource;

		public IInputElement InnerEventSource
		{
			get
			{
				return _innerEventSource;
			}
			private set
			{
				if (_innerEventSource != null)
				{
					UnregisterEvents();
				}
				_innerEventSource = value;
				if (_innerEventSource != null)
				{
					RegisterEvents(_innerEventSource);
				}
			}
		}

		public event EventHandler<DeviceEventArgs> DeviceDown;

		public event EventHandler<DeviceEventArgs> DeviceMove;

		public event EventHandler<DeviceEventArgs> DeviceUp;

		public DeviceEventSource(IInputElement inputElement)
		{
			InnerEventSource = inputElement;
		}

		public DeviceEventSource()
		{
		}

		public virtual void RegisterEvents(IInputElement inputElement)
		{
			if (InnerEventSource != null)
			{
				InnerEventSource.StylusDown += OnStylusDown;
				InnerEventSource.StylusMove += OnStylusMove;
				InnerEventSource.StylusUp += OnStylusUp;
				InnerEventSource.MouseLeftButtonDown += OnMouseDown;
				InnerEventSource.MouseMove += OnMouseMove;
				InnerEventSource.MouseLeftButtonUp += OnMouseUp;
			}
		}

		public virtual void UnregisterEvents()
		{
			if (InnerEventSource != null)
			{
				InnerEventSource.StylusDown -= OnStylusDown;
				InnerEventSource.StylusMove -= OnStylusMove;
				InnerEventSource.StylusUp -= OnStylusUp;
				InnerEventSource.MouseLeftButtonDown -= OnMouseDown;
				InnerEventSource.MouseMove -= OnMouseMove;
				InnerEventSource.MouseLeftButtonUp -= OnMouseUp;
			}
		}

		protected void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.StylusDevice == null)
			{
				Raise(DeviceDown, GetMouseDeviceEventArgs(e));
			}
		}

		protected void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.StylusDevice == null)
			{
				Raise(DeviceMove, GetMouseDeviceEventArgs(e));
			}
		}

		protected void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.StylusDevice == null)
			{
				Raise(DeviceUp, GetMouseDeviceEventArgs(e));
			}
		}

		protected void OnStylusDown(object sender, StylusEventArgs e)
		{
			Raise(DeviceDown, GetStylusDeviceEventArgs(e));
		}

		protected void OnStylusMove(object sender, StylusEventArgs e)
		{
			Raise(DeviceMove, GetStylusDeviceEventArgs(e));
		}

		protected void OnStylusUp(object sender, StylusEventArgs e)
		{
			Raise(DeviceUp, GetStylusDeviceEventArgs(e));
		}

		private DeviceEventArgs GetMouseDeviceEventArgs(MouseEventArgs e)
		{
			return new MouseDeviceEventArgs(e.GetPosition(InnerEventSource), e);
		}

		private DeviceEventArgs GetStylusDeviceEventArgs(StylusEventArgs e)
		{
			return new TouchDeviceEventArgs(e.GetPosition(InnerEventSource), e, e.StylusDevice.Id);
		}

		private void Raise<T>(EventHandler<T> eventDelegate, T e) where T : EventArgs
		{
			eventDelegate?.Invoke(this, e);
		}
	}
}
