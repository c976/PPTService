using System.Windows;
using Cvte.Windows.Mathematics;

namespace Cvte.Internal.Ink
{
	internal struct StrokeNodeData
	{
		private static StrokeNodeData s_empty;

		private Point _position;

		private float _pressure;

		internal static StrokeNodeData Empty => s_empty;

		internal bool IsEmpty => Nearly.Equals(_pressure, s_empty._pressure);

		internal Point Position => _position;

		internal float PressureFactor => _pressure;

		internal StrokeNodeData(Point position)
		{
			_position = position;
			_pressure = 1f;
		}

		internal StrokeNodeData(Point position, float pressure)
		{
			_position = position;
			_pressure = pressure;
		}
	}
}
