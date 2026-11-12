using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public static class ElementBoundsExtensions
	{
		[Obsolete("请使用 elements.GetOutlineExternalBounds 代替")]
		public static Rect TransformedBounds([NotNull] this IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			Rect empty = Rect.Empty;
			foreach (Element element in elements)
			{
				empty.Union(element.TransformedBounds());
			}
			return empty;
		}

		[Obsolete("请使用 element.Outline.GetExternalBounds 代替")]
		public static Rect TransformedBounds([NotNull] this Element element)
		{
			return element.Outline.GetExternalBounds();
		}

		[Obsolete("请使用 IEnumerable<Element>.GetOutlineExternalBounds 替代。")]
		public static Rect RigidExternalBounds([NotNull] this IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			return elements.GetOutlineExternalBounds();
		}

		[Obsolete("请使用 Element.Outline.GetExternalBounds 替代。")]
		public static Rect RigidExternalBounds([NotNull] this Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			return element.Outline.GetExternalBounds();
		}

		public static double TransformedRotation(this Element element)
		{
			return ((element.Rotation + GetAngle(element.RenderTransform.Value)) % 360.0 + 360.0) % 360.0;
		}

		private static double GetAngle(Matrix matrix)
		{
			Vector vector = new Vector(0.0, 1.0);
			Vector vector2 = matrix.Transform(new Vector(0.0, 1.0));
			return Vector.AngleBetween(vector, vector2);
		}

		public static void MoveElements(this IEnumerable<Element> elements, Vector offset)
		{
			IList<Element> list = elements as Element[];
			elements = list ?? (elements as IList<Element>) ?? elements.ToArray();
			if (!elements.Any())
			{
				return;
			}
			double contentScale = Board.GetContentScale(elements.First());
			if (contentScale <= 0.0)
			{
				return;
			}
			offset = new Vector(offset.X / contentScale, offset.Y / contentScale);
			foreach (Element element in elements)
			{
				Rect bounds = element.Bounds;
				bounds.Offset(offset);
				element.Bounds = bounds;
			}
		}
	}
}
