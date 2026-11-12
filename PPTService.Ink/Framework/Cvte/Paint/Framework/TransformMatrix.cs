using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	public class TransformMatrix
	{
		public delegate ValueTuple<Point, Point> CenterSpecification(Vector scalingFactor);

		public static class Centers
		{
			public static CenterSpecification ScaleAtZeroRotateAtCenter(Size originalSize)
			{
				return (Vector scalingFactor) => new ValueTuple<Point, Point>(default(Point), new Point(originalSize.Width * scalingFactor.X / 2.0, originalSize.Height * scalingFactor.Y / 2.0));
			}
		}

		public static class GroupGenerator
		{
			public static TransformGroup NoScaleButRotateAtOrigin(double rotation, Vector translation, Size originalSize)
			{
				return new TransformGroup
				{
					Children = 
					{
						(Transform)new RotateTransform
						{
							Angle = rotation
						},
						(Transform)new TranslateTransform
						{
							X = translation.X,
							Y = translation.Y
						}
					}
				};
			}

			public static TransformGroup ScaleAtZeroRotateAtCenter(Vector scaling, double rotation, Vector translation, Size originalSize, Point renderTransformOrigin = default(Point))
			{
				TransformGroup transformGroup = new TransformGroup();
				ScaleTransform value = new ScaleTransform
				{
					ScaleX = scaling.X,
					ScaleY = scaling.Y,
					CenterX = (0.0 - originalSize.Width) * renderTransformOrigin.X,
					CenterY = (0.0 - originalSize.Height) * renderTransformOrigin.Y
				};
				RotateTransform value2 = new RotateTransform
				{
					Angle = rotation,
					CenterX = originalSize.Width * (scaling.X / 2.0 - renderTransformOrigin.X),
					CenterY = originalSize.Height * (scaling.Y / 2.0 - renderTransformOrigin.Y)
				};
				transformGroup.Children.Add(value);
				transformGroup.Children.Add(value2);
				transformGroup.Children.Add(new TranslateTransform
				{
					X = translation.X,
					Y = translation.Y
				});
				return transformGroup;
			}
		}

		public static ValueTuple<Vector, double, Vector> MatrixToGroup(Matrix matrix, CenterSpecification specifyCenter = null)
		{
			Point[] array = new Point[4]
			{
				new Point(0.0, 0.0),
				new Point(1.0, 0.0),
				new Point(1.0, 1.0),
				new Point(0.0, 1.0)
			};
			Point[] array2 = array.Select(((Matrix)matrix).Transform).ToArray();
			Vector vector = new Vector((array2[1] - array2[0]).Length, (array2[3] - array2[0]).Length);
			double num = Vector.AngleBetween(new Vector(1.0, 0.0), array2[1] - array2[0]);
			Vector item = array2[0] - array[0];
			if (specifyCenter != null)
			{
				ValueTuple<Point, Point> valueTuple = specifyCenter(vector);
				Point item2 = valueTuple.Item1;
				Point item3 = valueTuple.Item2;
				Matrix identity = Matrix.Identity;
				identity.ScaleAt(vector.X, vector.Y, item2.X, item2.Y);
				Matrix identity2 = Matrix.Identity;
				identity2.RotateAt(num, item3.X, item3.Y);
				identity.Invert();
				identity2.Invert();
				Matrix trans = Matrix.Multiply(identity2, identity);
				trans = Matrix.Multiply(trans, matrix);
				item = new Vector(trans.OffsetX, trans.OffsetY);
			}
			return new ValueTuple<Vector, double, Vector>(vector, num, item);
		}
	}
}
