using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;
using Cvte.Paint.Framework.Controls;
using Cvte.Windows;

namespace Cvte.Paint.Framework
{
	public static class ElementHitTester
	{
		public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyProperty.RegisterAttached("IsHitTestVisible", typeof(bool), typeof(ElementHitTester), new PropertyMetadata(true));

		public static readonly DependencyProperty IsSlideHitTestVisibleProperty = DependencyProperty.RegisterAttached("IsSlideHitTestVisible", typeof(bool), typeof(ElementHitTester), new PropertyMetadata(true));

		private static bool GetIsHitTestVisible(DependencyObject element)
		{
			return (bool)element.GetValue(IsHitTestVisibleProperty);
		}

		public static bool GetIsHitTestVisible(Element element)
		{
			return (bool)element.GetValue(IsHitTestVisibleProperty);
		}

		public static void SetIsHitTestVisible(Element element, bool value)
		{
			element.SetValue(IsHitTestVisibleProperty, value);
		}

		private static bool GetIsSlideHitTestVisible([NotNull] Slide slide)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			return (bool)slide.GetValue(IsSlideHitTestVisibleProperty);
		}

		public static bool IsHitTestVisible([NotNull] Slide slide, ElementHitTestLevel level = ElementHitTestLevel.Normal)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			return GetIsSlideHitTestVisible(slide);
		}

		[Pure]
		[CanBeNull]
		public static Element HitElement([NotNull] this Slide slide, Point hitPoint, double allowance = 0.0, ElementHitTestLevel level = ElementHitTestLevel.Normal, bool hitInner = false)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			if (allowance < 0.0)
			{
				throw new ArgumentException("allowance");
			}
			Element hitResult = null;
			if (allowance > 0.0)
			{
				hitResult = slide.HitElement(hitPoint);
				if (hitResult != null)
				{
					return hitResult;
				}
			}
			VisualTreeHelper.HitTest(slide.Canvas, (DependencyObject target) =>
			{
				UIElement uIElement = target as UIElement;
				if (uIElement == null)
				{
					return HitTestFilterBehavior.ContinueSkipSelf;
				}
				return (((uIElement.IsHitTestVisible && GetIsHitTestVisible(uIElement)) || level != ElementHitTestLevel.Normal) && uIElement.IsVisible) ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
			}, (HitTestResult result) =>
			{
				if (hitInner)
				{
					hitResult = result.VisualHit as Element;
					if (hitResult == null)
					{
						hitResult = result.VisualHit.VisualAncestor<Element>();
					}
				}
				else
				{
					hitResult = result.VisualHit.TopGroupElement();
				}
				return (hitResult == null) ? HitTestResultBehavior.Continue : HitTestResultBehavior.Stop;
			}, (allowance < 0.5) ? ((HitTestParameters)new PointHitTestParameters(hitPoint)) : ((HitTestParameters)new GeometryHitTestParameters(new EllipseGeometry(hitPoint, allowance, allowance))));
			return hitResult;
		}

		[Pure]
		[CanBeNull]
		public static Element[] HitElements([NotNull] this Slide slide, Rect hitBounds)
		{
			return slide.HitElements(new RectangleGeometry(hitBounds));
		}

		[Pure]
		[CanBeNull]
		public static Element[] HitElements([NotNull] this Slide slide, Geometry hitGeometry)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			List<Element> hitResults = new List<Element>();
			List<IElementPartHittable> partResults = new List<IElementPartHittable>();
			VisualTreeHelper.HitTest(slide.Canvas, (DependencyObject target) =>
			{
				if (target is Viewport3D)
				{
					return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
				}
				IElementPartHittable elementPartHittable = target as IElementPartHittable;
				if (elementPartHittable != null)
				{
					partResults.Add(elementPartHittable);
				}
				UIElement uIElement = target as UIElement;
				if (uIElement == null)
				{
					return HitTestFilterBehavior.ContinueSkipSelf;
				}
				return (uIElement.IsHitTestVisible && GetIsHitTestVisible(uIElement) && uIElement.IsVisible) ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
			}, (HitTestResult result) =>
			{
				Element element = result.VisualHit.TopGroupElement();
				if (element != null && element.IsHitTestVisible && GetIsHitTestVisible(element) && element.Visibility == Visibility.Visible && !hitResults.Contains(element))
				{
					hitResults.Add(element);
				}
				return HitTestResultBehavior.Continue;
			}, new GeometryHitTestParameters(hitGeometry));
			List<object> hittedObjects = partResults.Select((IElementPartHittable child) => child.HittedObject).ToList();
			partResults.ForEach((IElementPartHittable child) =>
			{
				child.HitPart(hittedObjects);
			});
			return hitResults.ToArray();
		}

		[Pure]
		[CanBeNull]
		public static InteractiveButton HitInteractiveButton([NotNull] Slide slide, Point hitPoint, ElementHitTestLevel level = ElementHitTestLevel.Normal)
		{
			if (slide == null)
			{
				throw new ArgumentNullException("slide");
			}
			InteractiveButton hitResult = null;
			VisualTreeHelper.HitTest(slide.Canvas, (DependencyObject target) =>
			{
				UIElement uIElement = target as UIElement;
				if (uIElement == null)
				{
					return HitTestFilterBehavior.ContinueSkipSelf;
				}
				InteractiveButton interactiveButton = uIElement as InteractiveButton;
				if (interactiveButton != null)
				{
					hitResult = interactiveButton;
				}
				else
				{
					hitResult = null;
				}
				if (((!uIElement.IsHitTestVisible || !GetIsHitTestVisible(uIElement)) && level == ElementHitTestLevel.Normal) || !uIElement.IsVisible)
				{
					return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
				}
				return (hitResult == null) ? HitTestFilterBehavior.Continue : HitTestFilterBehavior.Stop;
			}, (HitTestResult result) =>
			{
				InteractiveButton interactiveButton = result.VisualHit as InteractiveButton;
				if (interactiveButton != null)
				{
					hitResult = interactiveButton;
					return HitTestResultBehavior.Stop;
				}
				hitResult = result.VisualHit.VisualAncestor<InteractiveButton>();
				return (hitResult == null) ? HitTestResultBehavior.Continue : HitTestResultBehavior.Stop;
			}, new PointHitTestParameters(hitPoint));
			return hitResult;
		}
	}
}
