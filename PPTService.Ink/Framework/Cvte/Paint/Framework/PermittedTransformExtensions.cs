using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public static class PermittedTransformExtensions
	{
		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass0_0
		{
			public Element element;
		}

		public static readonly RoutedEvent PermittedTransformInvalidatedEvent = EventManager.RegisterRoutedEvent("PermittedTransformInvalidatedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PermittedTransformExtensions));

		public static PermittedTransforms GetPermittedTransform([NotNull] this Element element)
		{
			_003C_003Ec__DisplayClass0_0 obj = default(_003C_003Ec__DisplayClass0_0);
			obj.element = element;
			if (obj.element == null)
			{
				throw new ArgumentNullException("element");
			}
			PermittedTransforms permittedTransforms = _003CGetPermittedTransform_003Eg__GetElementPermittedTransform_007C0_0(ref obj);
			if (permittedTransforms.HasFlag(PermittedTransforms.FixedRatioFlag))
			{
				permittedTransforms &= ~PermittedTransforms.CenterScale;
			}
			return permittedTransforms;
		}

		public static PermittedTransforms GetPermittedTransform([NotNull] this IEnumerable<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException("elements");
			}
			return elements.Select((Element x) => x.GetPermittedTransform()).ToArray().GetPermittedTransform();
		}

		public static PermittedTransforms GetPermittedTransform([NotNull] this PermittedTransforms[] permits)
		{
			if (permits == null)
			{
				throw new ArgumentNullException("permits");
			}
			PermittedTransforms permittedTransforms = permits.Aggregate(PermittedTransforms.FreeRenderTransform, (PermittedTransforms current, PermittedTransforms permit) => current & permit);
			bool flag = permits.Any((PermittedTransforms permit) => permit.HasFlag(PermittedTransforms.FixedRatioFlag));
			bool flag2 = permits.Any((PermittedTransforms permit) => permit.HasFlag(PermittedTransforms.FixedRatioByDefaultFlag));
			bool flag3 = permits.All((PermittedTransforms permit) => permit.HasFlag(PermittedTransforms.FlippingFlag));
			if (flag)
			{
				permittedTransforms |= PermittedTransforms.FixedRatioFlag;
				permittedTransforms &= ~PermittedTransforms.CenterScale;
			}
			else if (flag2)
			{
				permittedTransforms |= PermittedTransforms.FixedRatioByDefaultFlag;
			}
			if (flag3)
			{
				permittedTransforms |= PermittedTransforms.FlippingFlag;
			}
			return permittedTransforms;
		}

		public static void InvalidatePermittedTransform<T>(this T element) where T : Element, IPermittedTransform
		{
			Slide.GetSlide(element)?.RaiseEvent(new RoutedEventArgs
			{
				RoutedEvent = PermittedTransformInvalidatedEvent
			});
		}

		[CompilerGenerated]
		private static PermittedTransforms _003CGetPermittedTransform_003Eg__GetElementPermittedTransform_007C0_0(ref _003C_003Ec__DisplayClass0_0 P_0)
		{
			return (P_0.element as IPermittedTransform)?.PermittedTransform ?? ((TransformAttribute)P_0.element.GetType().GetCustomAttributes(typeof(TransformAttribute), true).FirstOrDefault())?.Permitted ?? PermittedTransforms.FreeRenderTransform;
		}
	}
}
