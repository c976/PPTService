using System;
using System.Collections.Generic;
using System.Windows;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Framework
{
	public static class TransformHelper
	{
		public static readonly DependencyProperty TransformMinimumSizeProperty = DependencyProperty.RegisterAttached("TransformMinimumSize", typeof(Size), typeof(Element), new PropertyMetadata(new Size(2.0, 2.0)));

		public static Size GetTransformMinimumSize(this IEnumerable<Element> elements)
		{
			int num = 0;
			double num2 = double.PositiveInfinity;
			double num3 = double.PositiveInfinity;
			foreach (Element element in elements)
			{
				num++;
				Size transformMinimumSize = element.GetTransformMinimumSize();
				num2 = Math.Min(transformMinimumSize.Width, num2);
				num3 = Math.Min(transformMinimumSize.Height, num3);
			}
			if (num == 0)
			{
				throw new ArgumentException("变换的元素数量为0", "elements");
			}
			return new Size(num2, num3);
		}

		public static Rect GetValidElementBounds(this Element element, Rect bounds)
		{
			Size transformMinimumSize = element.GetTransformMinimumSize();
			double width = transformMinimumSize.Width;
			double height = transformMinimumSize.Height;
			double num = ((bounds.Height > height) ? bounds.Height : height);
			double num2 = ((bounds.Width > width) ? bounds.Width : width);
			Point point = bounds.Center();
			double num3 = point.X - num2 / 2.0;
			double num4 = point.Y - num / 2.0;
			if (num3 < 0.0)
			{
				num3 = 0.0;
			}
			if (num4 < 0.0)
			{
				num4 = 0.0;
			}
			return new Rect(num3, num4, num2, num);
		}

		public static Size GetTransformMinimumSize(this Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return (Size)element.GetValue(TransformMinimumSizeProperty);
		}

		public static void SetTransformMinimumSize(this Element element, Size value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			element.SetValue(TransformMinimumSizeProperty, value);
		}
	}
}
