using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public class PolyLineGeometryBuilder
	{
		private PathGeometry _pathGeometry;

		private PolyLineSegment _polyLineSegment;

		public Geometry Geometry => _pathGeometry;

		public bool IsEmpty
		{
			get
			{
				PolyLineSegment polyLineSegment = _polyLineSegment;
				if (polyLineSegment == null)
				{
					return true;
				}
				return !polyLineSegment.Points.Any();
			}
		}

		public bool Closed { get; }

		public PolyLineGeometryBuilder(bool closed = true)
		{
			Closed = closed;
		}

		public void Add(Point point)
		{
			if (_pathGeometry == null)
			{
				_polyLineSegment = new PolyLineSegment();
				_pathGeometry = new PathGeometry(new PathFigureCollection
				{
					new PathFigure(point, new PolyLineSegment[1] { _polyLineSegment }, Closed)
				});
			}
			_polyLineSegment.Points.Add(point);
		}

		public void Reset()
		{
			_pathGeometry = null;
		}
	}
}
