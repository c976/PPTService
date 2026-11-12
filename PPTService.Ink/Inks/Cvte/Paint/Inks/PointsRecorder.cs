using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Cvte.Paint.Inks
{
	internal class PointsRecorder
	{
		private class StrokePointsRecorder
		{
			public DateTime CreationTime { get; }

			public DateTime FinishedTime { get; set; }

			public List<StylusPoint> StrokePoints { get; }

			public StrokePointsRecorder()
			{
				CreationTime = DateTime.Now;
				StrokePoints = new List<StylusPoint>();
			}
		}

		private readonly Dictionary<int, StrokePointsRecorder> _strokes = new Dictionary<int, StrokePointsRecorder>();

		internal void RecordPoints(int id, IEnumerable<StylusPoint> points)
		{
			if (!_strokes.ContainsKey(id))
			{
				_strokes[id] = new StrokePointsRecorder();
			}
			_strokes[id].StrokePoints.AddRange(points);
			_strokes[id].FinishedTime = DateTime.Now;
		}

		internal StylusPointCollection GetRecordedPoints(int id)
		{
			return new StylusPointCollection(_strokes[id].StrokePoints.Select((StylusPoint p) => new StylusPoint(p.X, p.Y, p.PressureFactor)));
		}

		internal DateTime GetStrokeCreationTime(int id)
		{
			return _strokes[id].CreationTime;
		}

		internal void Remove(int id)
		{
			if (_strokes.ContainsKey(id))
			{
				_strokes.Remove(id);
			}
		}

		internal void Clear()
		{
			_strokes.Clear();
		}
	}
}
