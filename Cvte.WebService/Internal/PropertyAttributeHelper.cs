using System;
using System.Linq;
using System.Reflection;

namespace Cvte.WebService.Internal
{
	internal static class PropertyAttributeHelper<T> where T : Attribute
	{
		public static T smethod_0(PropertyInfo propertyInfo_0, bool bool_0 = false)
		{
			T[] source = smethod_1(propertyInfo_0, bool_0);
			if (source.Any())
			{
				return source.First();
			}
			return null;
		}

		public static T[] smethod_1(PropertyInfo propertyInfo_0, bool bool_0 = false)
		{
			return (T[])propertyInfo_0.GetCustomAttributes(typeof(T), bool_0);
		}
	}
}
