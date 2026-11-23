using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Ink.Core.Windows
{
	public static class VisualScalingExtensions
	{
		public static Size GetScalingRatioToDevice(this Visual visual)
		{
			return visual.GetTransformInfoToDevice().Item1;
		}

		
		public static ValueTuple<Size, double> GetTransformInfoToDevice([NotNull] this Visual visual)
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			Visual ancestor = VisualRoot(visual);
			Matrix value = ((MatrixTransform)visual.TransformToAncestor(ancestor)).Value;
			CompositionTarget compositionTarget = PresentationSource.FromVisual(visual)?.CompositionTarget;
			if (compositionTarget != null)
			{
				value.Append(compositionTarget.TransformToDevice);
			}
			Vector vector = new Vector(1.0, 0.0);
			Vector vector2 = value.Transform(vector);
			double num = Vector.AngleBetween(vector, vector2);
			value.Rotate(0.0 - num);
			Rect rect = new Rect(new Size(1.0, 1.0));
			rect.Transform(value);
			return new ValueTuple<Size, double>(rect.Size, num);
		}

		internal static Size GetScaleSize(Matrix transform)
		{
			Vector vector = new Vector(1.0, 0.0);
			Vector vector2 = transform.Transform(vector);
			double num = Vector.AngleBetween(vector, vector2);
			transform.Rotate(0.0 - num);
			Rect rect = new Rect(new Size(1.0, 1.0));
			rect.Transform(transform);
			return rect.Size;
		}

		[NotNull]
		private static Visual VisualRoot([NotNull] Visual visual)
		{
			if (visual == null)
			{
				throw new ArgumentNullException("visual");
			}
			Visual result = visual;
			for (DependencyObject parent = VisualTreeHelper.GetParent(visual); parent != null; parent = VisualTreeHelper.GetParent(parent))
			{
				Visual visual2 = parent as Visual;
				if (visual2 != null)
				{
					result = visual2;
				}
			}
			return result;
		}
	}
}
