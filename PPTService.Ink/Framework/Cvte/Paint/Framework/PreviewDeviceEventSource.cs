using System.Windows;

namespace Cvte.Paint.Framework
{
	internal class PreviewDeviceEventSource : DeviceEventSource
	{
		public PreviewDeviceEventSource(IInputElement inputElement)
			: base(inputElement)
		{
		}

		public sealed override void RegisterEvents(IInputElement inputElement)
		{
			if (InnerEventSource != null)
			{
				InnerEventSource.PreviewMouseLeftButtonDown += OnMouseDown;
				InnerEventSource.PreviewMouseMove += OnMouseMove;
				InnerEventSource.PreviewMouseLeftButtonUp += OnMouseUp;
				InnerEventSource.PreviewStylusButtonDown += OnStylusDown;
				InnerEventSource.PreviewStylusMove += OnStylusMove;
				InnerEventSource.PreviewStylusButtonUp += OnStylusUp;
			}
		}

		public sealed override void UnregisterEvents()
		{
			if (InnerEventSource != null)
			{
				InnerEventSource.PreviewMouseLeftButtonDown -= OnMouseDown;
				InnerEventSource.PreviewMouseMove -= OnMouseMove;
				InnerEventSource.PreviewMouseLeftButtonUp -= OnMouseUp;
				InnerEventSource.PreviewStylusButtonDown -= OnStylusDown;
				InnerEventSource.PreviewStylusMove -= OnStylusMove;
				InnerEventSource.PreviewStylusButtonUp -= OnStylusUp;
			}
		}
	}
}
