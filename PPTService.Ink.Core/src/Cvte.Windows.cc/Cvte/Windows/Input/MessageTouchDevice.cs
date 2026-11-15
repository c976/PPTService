using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PPTService.Ink.Core.Windows.Input
{
	internal class MessageTouchDevice : TouchDevice
	{
		public Point Position { get; set; }

		public Size Size { get; set; }

		public Window Window { get; }

		public TouchAction TouchAction { get; set; }

		public MessageTouchDevice(int deviceId, Window window)
			: base(deviceId)
		{
			Window = window;
			window.Closed += Window_Closed;
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			MessageTouchDeviceHelper.Remove(sender as Window);
		}

		public override TouchPoint GetTouchPoint(IInputElement relativeTo)
		{
			Point point = Position;
			if (relativeTo != null && ActiveSource != null)
			{
				point = ActiveSource.RootVisual.TransformToDescendant((Visual)relativeTo).Transform(Position);
			}
			return new TouchPoint(this, point, new Rect(point, Size), TouchAction);
		}

		protected override void OnCapture(IInputElement element, CaptureMode captureMode)
		{
			Mouse.PrimaryDevice.Capture(element, captureMode);
		}

		public override TouchPointCollection GetIntermediateTouchPoints(IInputElement relativeTo)
		{
			return new TouchPointCollection { GetTouchPoint(relativeTo) };
		}

		public void Down()
		{
			TouchAction = TouchAction.Down;
			if (!IsActive)
			{
				SetActiveSource(PresentationSource.FromVisual(Window));
				Activate();
				ReportDown();
			}
			else
			{
				ReportDown();
			}
		}

		public void Move()
		{
			TouchAction = TouchAction.Move;
			ReportMove();
		}

		public void Up()
		{
			TouchAction = TouchAction.Up;
			ReportUp();
			Deactivate();
		}
	}
}
