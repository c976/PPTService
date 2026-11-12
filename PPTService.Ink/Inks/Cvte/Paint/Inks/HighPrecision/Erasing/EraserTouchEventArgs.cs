using System;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Cvte.Paint.Inks.HighPrecision.Erasing
{
	public struct EraserTouchEventArgs : IEquatable<EraserTouchEventArgs>
	{
		public int Id { get; }

		public double X { get; }

		public double Y { get; }

		public double Width { get; }

		public double Height { get; }

		public double Rotation { get; }

		public double SlideScaleX { get; }

		public double SlideScaleY { get; }

		public Point Position => new Point(X, Y);

		public Size Size => new Size(Width, Height);

		public Rect Rect => new Rect(X, Y, Width, Height);

		public EraserTouchEventArgs(int id, double x, double y, double width, double height, double rotation, double scaleX, double scaleY)
		{
			Id = id;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			Rotation = rotation;
			SlideScaleX = scaleX;
			SlideScaleY = scaleY;
		}

		public bool Equals(EraserTouchEventArgs other)
		{
			if (Id == other.Id && X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height) && Rotation.Equals(other.Rotation) && SlideScaleX.Equals(other.SlideScaleX))
			{
				return SlideScaleY.Equals(other.SlideScaleY);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is EraserTouchEventArgs)
			{
				EraserTouchEventArgs other = (EraserTouchEventArgs)obj;
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (((((((((((((Id * 397) ^ X.GetHashCode()) * 397) ^ Y.GetHashCode()) * 397) ^ Width.GetHashCode()) * 397) ^ Height.GetHashCode()) * 397) ^ Rotation.GetHashCode()) * 397) ^ SlideScaleX.GetHashCode()) * 397) ^ SlideScaleY.GetHashCode();
		}
	}
}
