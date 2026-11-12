using System.Windows;

namespace Cvte.Internal.Ink
{
	internal struct ContourSegment
	{
		private Point _begin;

		private Vector _vector;

		private Vector _radius;

		internal bool IsArc
		{
			get
			{
				if (_radius.X == 0.0)
				{
					return _radius.Y != 0.0;
				}
				return true;
			}
		}

		internal Point Begin => _begin;

		internal Point End => _begin + _vector;

		internal Vector Vector => _vector;

		internal Vector Radius => _radius;

		internal ContourSegment(Point begin, Point end)
		{
			_begin = begin;
			_vector = (DoubleUtil.AreClose(begin, end) ? new Vector(0.0, 0.0) : (end - begin));
			_radius = new Vector(0.0, 0.0);
		}

		internal ContourSegment(Point begin, Point end, Point center)
		{
			_begin = begin;
			_vector = end - begin;
			_radius = center - begin;
		}
	}
}
