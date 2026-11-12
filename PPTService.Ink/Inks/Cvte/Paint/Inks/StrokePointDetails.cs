using System;
using System.Windows.Input;

namespace Cvte.Paint.Inks
{
	public class StrokePointDetails
	{
		private DateTime _time;

		public double X { get; }

		public double Y { get; }

		public float PressureFactor { get; }

		public DateTime Time
		{
			get
			{
				if (_time == default(DateTime))
				{
					int tickCount = Environment.TickCount;
					DateTime now = DateTime.Now;
					int num = tickCount - EnvironmentTickCount;
					_time = now.AddMilliseconds(-1 * num);
				}
				return _time;
			}
		}

		internal int EnvironmentTickCount { get; }

		public StrokePointDetails(StylusPoint point)
		{
			X = point.X;
			Y = point.Y;
			PressureFactor = point.PressureFactor;
			EnvironmentTickCount = Environment.TickCount;
		}

		public StrokePointDetails(double x, double y, float pressureFactor, DateTime time)
		{
			X = x;
			Y = y;
			PressureFactor = pressureFactor;
			_time = time;
		}

		public override string ToString()
		{
			return $"x={X:0.00},y={Y:0.00},time={Time:hh:mm:ss}";
		}
	}
}
