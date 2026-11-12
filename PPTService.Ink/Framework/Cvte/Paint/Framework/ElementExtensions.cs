using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.GeometryMath;
using Cvte.Windows.Mathematics.Geometries;

namespace Cvte.Paint.Framework
{
	public static class ElementExtensions
	{
		public static void InnerSelect<T>(this Element element, object parent, params T[] selectedItems)
		{
			element.RaiseEvent(new RoutedInnerCollectionChangedEventArgs<object>(Slide.InnerSelectionChangedEvent, element, parent, ((IEnumerable<T>)selectedItems).Select((Func<T, object>)((T item) => item)).ToList()));
		}

		public static Point GetVisualOriginPoint(Rect bounds, double rotation, Point logicalOrigin)
		{
			Point point = GeometryCalculatorExtensions.Rotate(origin: new Point(bounds.Width / 2.0, bounds.Height / 2.0), point: new Point(logicalOrigin.X * bounds.Width, logicalOrigin.Y * bounds.Height), angle: rotation);
			return new Point(point.X / bounds.Width, point.Y / bounds.Height);
		}

		public static Point GetLogicalOriginPoint(Rect bounds, double rotation, Point visualOrigin)
		{
			Point point = new Point(bounds.Width / 2.0, bounds.Height / 2.0);
			Point point2 = GeometryCalculatorExtensions.GetRotateVector(new Point(bounds.Width * visualOrigin.X, bounds.Height * visualOrigin.Y) - point, 0.0 - rotation) + point;
			return new Point(point2.X / bounds.Width, point2.Y / bounds.Height);
		}

		public static void RotateAt(this Element element, double angle, Point originInSlide)
		{
			Rect bounds = element.Bounds;
			Point point = bounds.Center();
			Vector vector = Geometry2D.Rotate(point - originInSlide, 0.0 - angle) + originInSlide - point;
			element.Location = bounds.Location + vector;
			element.Rotation += angle;
		}

		public static Point OriginToSlide(this Element element, Point visualOrigin)
		{
			Rect bounds = element.Bounds;
			return new Point(element.Location.X + bounds.Width * visualOrigin.X, element.Location.Y + bounds.Height * visualOrigin.Y);
		}

		public static void RotateAtOrigin(this Element element, double angle)
		{
			Point visualOriginPoint = GetVisualOriginPoint(element.Bounds, element.Rotation, element.LogicalOriginPoint);
			Point originInSlide = element.OriginToSlide(visualOriginPoint);
			element.RotateAt(angle, originInSlide);
		}

		[Pure]
		[CanBeNull]
		public static T GetCounterpart<T>(this T element) where T : Element
		{
			T counterpartWithoutOffset = element.GetCounterpartWithoutOffset();
			Slide slide = Slide.GetSlide(element);
			if (slide != null && counterpartWithoutOffset != null)
			{
				Point point = element.TranslatePoint(default(Point), slide);
				double offsetX = counterpartWithoutOffset.RenderTransform.Value.OffsetX;
				double offsetY = counterpartWithoutOffset.RenderTransform.Value.OffsetY;
				Vector offsetVector = point - new Point(offsetX, offsetY);
				Rect bounds = counterpartWithoutOffset.Bounds;
				bounds.Offset(offsetVector);
				counterpartWithoutOffset.Bounds = bounds;
			}
			return counterpartWithoutOffset;
		}

		[Pure]
		[CanBeNull]
		public static T GetCounterpartWithoutOffset<T>(this T element) where T : Element
		{
			Element element2 = element.GetElementSaveInfo()?.Clone().ToElement();
			if (element2 == null)
			{
				return null;
			}
			return (T)element2;
		}
	}
}
