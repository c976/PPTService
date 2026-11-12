using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public static class ElementExtensionHelper
	{
		[CanBeNull]
		public static T GetExtension<T>(this Element element) where T : IExtension<Element>
		{
			return element.Extensions.GetExtension<T>();
		}
	}
}
