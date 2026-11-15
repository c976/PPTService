using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PPTService.Ink.Core.Windows.Input
{
	public class DeviceInputEventArgs : EventArgs
	{
		private IEnumerable<StylusPoint> _stylusPoints;

		public DeviceType Type { get; private set; }

		public VirtualDeviceType VirtualType { get; set; }

		public DeviceButton Button { get; set; }

		public int Id { get; private set; }

		public Point Position { get; private set; }

		public InputEventArgs OriginEventArgs { get; private set; }

		protected DeviceInputEventArgs(int id, DeviceType type, DeviceButton button)
		{
			Type = type;
			Button = button;
			Id = id;
		}

		public DeviceInputEventArgs(int id, Point position, DeviceType type, DeviceButton button, InputEventArgs originInputEventArgs)
		{
			OriginEventArgs = originInputEventArgs;
			Id = id;
			Position = position;
			Type = type;
			Button = button;
		}

		[Obsolete("请使用DeviceInputEventArgs(int id, Point position, DeviceType type, DeviceButton button, IEnumerable<StylusPoint> stylusPoints)的版本")]
		public DeviceInputEventArgs(int id, Point position, DeviceType type, DeviceButton button, IEnumerable<Point> stylusPoints, InputEventArgs originInputEventArgs)
		{
			OriginEventArgs = originInputEventArgs;
			Id = id;
			Position = position;
			Type = type;
			Button = button;
			_stylusPoints = stylusPoints?.Select((Point p) => new StylusPoint(p.X, p.Y)).ToArray() ?? new StylusPoint[1]
			{
				new StylusPoint(position.X, position.Y)
			};
		}

		public DeviceInputEventArgs(int id, Point position, DeviceType type, DeviceButton button, IEnumerable<StylusPoint> stylusPoints, InputEventArgs originInputEventArgs)
		{
			Id = id;
			OriginEventArgs = originInputEventArgs;
			Position = position;
			Type = type;
			Button = button;
			_stylusPoints = stylusPoints;
		}

		[Obsolete("请使用GetStylusPoints")]
		public IEnumerable<Point> GetTouchPoints()
		{
			return from x in GetStylusPoints()
				select x.ToPoint();
		}

		public IEnumerable<StylusPoint> GetStylusPoints()
		{
			if (_stylusPoints == null)
			{
				_stylusPoints = new StylusPoint[1]
				{
					new StylusPoint(Position.X, Position.Y)
				};
			}
			return _stylusPoints;
		}
	}
}
