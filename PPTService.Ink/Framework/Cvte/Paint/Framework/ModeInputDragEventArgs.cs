using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;
using Cvte.Windows.Input;

namespace Cvte.Paint.Framework
{
	public class ModeInputDragEventArgs : ModeInputEventArgs, ICloneable
	{
		public Point OriginPosition { get; }

		public Vector OffsetToOrigin { get; }

		public Vector OffsetToLast { get; }

		public ModeInputDragEventArgs([CanBeNull] InputEventArgs args, int id, DeviceType type, DeviceButton button, Point originPosition, Point currentPosition, Vector offsetToOrigin, Vector offsetToLast, IEnumerable<StylusPoint> stylusPoints)
			: base(args, id, currentPosition, type, button, stylusPoints)
		{
			OriginPosition = originPosition;
			OffsetToOrigin = offsetToOrigin;
			OffsetToLast = offsetToLast;
		}

		public static ModeInputDragEventArgs FromDeviceInput(DeviceInputEventArgs e, Point originPosition, Point currentPosition, Vector offsetToOrigin, Vector offsetToLast, DeviceType? type = null, DeviceButton? button = null)
		{
			DeviceType type2 = type ?? e.Type;
			DeviceButton button2 = button ?? e.Button;
			ModeInputDragEventArgs e2 = new ModeInputDragEventArgs(e.OriginEventArgs, e.Id, type2, button2, originPosition, currentPosition, offsetToOrigin, offsetToLast, e.GetStylusPoints())
			{
				VirtualType = e.VirtualType
			};
			ModeInputEventArgs e3 = e as ModeInputEventArgs;
			if (e3 != null)
			{
				e2.HitTestProvider = e3.HitTestProvider;
			}
			return e2;
		}
	}
}
