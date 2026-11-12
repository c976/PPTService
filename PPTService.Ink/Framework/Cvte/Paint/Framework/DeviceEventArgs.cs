using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Cvte.Paint.Framework
{
	public abstract class DeviceEventArgs : EventArgs
	{
		private const int TraceForCurrentPointsCount = 100;

		public InputEventArgs OrignalArgs { get; private set; }

		public Point Position { get; internal set; }

		public abstract bool IsTouch { get; }

		public bool IsPressing
		{
			get
			{
				if (!IsTouch)
				{
					return Mouse.LeftButton == MouseButtonState.Pressed;
				}
				return true;
			}
		}

		public int DeviceId { get; private set; }

		public List<Point> Trace { get; private set; }

		public List<Point> TraceForCurrentPoints { get; private set; }

		protected DeviceEventArgs(Point position, InputEventArgs args, int deviceId)
		{
			Position = position;
			DeviceId = deviceId;
			OrignalArgs = args;
			Trace = new List<Point> { position, position };
			TraceForCurrentPoints = new List<Point> { position, position };
		}

		public void RecordTrackPoint(Point position)
		{
			Position = position;
			if (Trace.Count >= 2)
			{
				Trace.RemoveAt(1);
			}
			Trace.Add(position);
		}

		public void RecordTrackPointForPreAndCur(Point position)
		{
			Position = position;
			TraceForCurrentPoints.Add(position);
			if (TraceForCurrentPoints.Count > 100)
			{
				TraceForCurrentPoints.RemoveAt(0);
			}
		}

		public void ClearRecordTrack()
		{
			Trace.Clear();
			TraceForCurrentPoints.Clear();
		}

		public double GetTracePointsDistance()
		{
			return (Trace.LastOrDefault() - Trace.FirstOrDefault()).Length;
		}

		public bool IsTraceForCurrentPointsDistance(double distance, bool removeTrace)
		{
			bool result = false;
			if ((TraceForCurrentPoints.LastOrDefault() - TraceForCurrentPoints.FirstOrDefault()).Length > distance)
			{
				result = true;
				if (removeTrace)
				{
					TraceForCurrentPoints.RemoveRange(0, TraceForCurrentPoints.Count - 1);
				}
			}
			return result;
		}

		public Vector GetTraceSlideVector()
		{
			return Trace.LastOrDefault() - Trace.FirstOrDefault();
		}
	}
}
