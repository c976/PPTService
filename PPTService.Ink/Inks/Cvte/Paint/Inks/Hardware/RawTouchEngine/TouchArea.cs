using System;
using System.Windows;

namespace Cvte.Paint.Inks.Hardware.RawTouchEngine
{
	public class TouchArea
	{
		internal Point Position { get; set; }

		public Size Size { get; internal set; }

		public DateTime CreatedTime { get; internal set; }

		public DateTime LastUpdatedTime { get; internal set; }

		public override string ToString()
		{
			return $"TouchArea{{ Position={Position}, Size={Size}}}";
		}
	}
}
