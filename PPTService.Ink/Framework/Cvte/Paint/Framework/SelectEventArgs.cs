using System.Collections.Generic;
using System.Windows;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public class SelectEventArgs : ModeInputDragEventArgs
	{
		public List<string> SelectedElementIds { get; internal set; }

		public string AdornerId { get; internal set; }

		public Point PointInSlide { get; internal set; }

		public SelectEventArgs(ModeInputDragEventArgs modeInputArgs)
			: this(modeInputArgs.Id, modeInputArgs.Type, modeInputArgs.Button, modeInputArgs.OriginPosition, modeInputArgs.Position, modeInputArgs.OffsetToOrigin, modeInputArgs.OffsetToLast)
		{
		}

		public SelectEventArgs(int id, DeviceType type, DeviceButton button, Point originPosition, Point currentPosition, Vector offsetToOrigin, Vector offsetToLast)
			: base(null, id, type, button, originPosition, currentPosition, offsetToOrigin, offsetToLast, null)
		{
			SelectedElementIds = new List<string>();
		}
	}
}
