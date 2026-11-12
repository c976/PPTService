using System.Collections.Generic;
using System.Windows;

namespace Cvte.Paint.Framework
{
	public interface ITransformAdapter
	{
		IList<Element> Elements { get; }

		Rect Bounds { get; }

		double Rotation { get; }

		Rect DesiredBounds { get; }

		double DesiredRotation { get; }

		void StartTransform(TransformRoutedEventArgs args);

		void CompleteTransform(TransformRoutedEventArgs args);

		void PerformTransform(Rect targetBounds, object[] args);

		void PerformTransform(Vector movedOffset, object[] args);

		void PerformTransform(double angle, object[] args);

		void Apply(Rect finalBounds, double finalRotation);
	}
}
