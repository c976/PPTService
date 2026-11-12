using System;
using System.Linq;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public static class ExtensionCollectionHelper
	{
		public static TExtension GetSingleExtension<T, TExtension>([NotNull] this ExtensionCollection<T> extensions) where T : IExtensible<T> where TExtension : IExtension<T>
		{
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions");
			}
			return extensions.OfType<TExtension>().SingleOrDefault();
		}

		public static TExtension GetSingleExtension<TExtension>([NotNull] this ExtensionCollection<Element> extensions) where TExtension : IExtension<Element>
		{
			if (extensions == null)
			{
				throw new ArgumentNullException("extensions");
			}
			return extensions.OfType<TExtension>().SingleOrDefault();
		}
	}
}
