using System;
using System.Collections.Generic;
using System.Windows;

namespace Cvte.Internal.Ink
{
	internal struct Quad
	{
		private static Quad s_empty = new Quad(new Point(0.0, 0.0), new Point(0.0, 0.0), new Point(0.0, 0.0), new Point(0.0, 0.0));

		private Point _A;

		private Point _B;

		private Point _C;

		private Point _D;

		internal static Quad Empty => s_empty;

		internal Point A
		{
			get
			{
				return _A;
			}
			set
			{
				_A = value;
			}
		}

		internal Point B
		{
			get
			{
				return _B;
			}
			set
			{
				_B = value;
			}
		}

		internal Point C
		{
			get
			{
				return _C;
			}
			set
			{
				_C = value;
			}
		}

		internal Point D
		{
			get
			{
				return _D;
			}
			set
			{
				_D = value;
			}
		}

		internal Point this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return _A;
				case 1:
					return _B;
				case 2:
					return _C;
				case 3:
					return _D;
				default:
					throw new IndexOutOfRangeException("index");
				}
			}
		}

		internal bool IsEmpty
		{
			get
			{
				if (_A == _B)
				{
					return _C == _D;
				}
				return false;
			}
		}

		internal Rect Bounds
		{
			get
			{
				if (!IsEmpty)
				{
					return Rect.Union(new Rect(_A, _B), new Rect(_C, _D));
				}
				return Rect.Empty;
			}
		}

		internal Quad(Point a, Point b, Point c, Point d)
		{
			_A = a;
			_B = b;
			_C = c;
			_D = d;
		}

		internal void GetPoints(List<Point> pointBuffer)
		{
			pointBuffer.Add(_A);
			pointBuffer.Add(_B);
			pointBuffer.Add(_C);
			pointBuffer.Add(_D);
		}
	}
}
