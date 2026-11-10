using System;
using System.Linq;
using System.Reflection;

namespace Cvte.Pipe.Http.Internal
{
	internal static class AttributeHelper<T> where T : Attribute
	{
		public static bool smethod_0(object object_0, bool bool_0 = false)
		{
			return smethod_1(object_0.GetType());
		}

		public static bool smethod_1(Type type_0, bool bool_0 = false)
		{
			return type_0.IsDefined(typeof(T), bool_0);
		}

		public static bool smethod_2(PropertyInfo propertyInfo_0, bool bool_0 = false)
		{
			return propertyInfo_0.IsDefined(typeof(T), bool_0);
		}

		public static T smethod_3(object object_0, bool bool_0 = false)
		{
			return smethod_4(object_0.GetType());
		}

		public static T smethod_4(Type type_0, bool bool_0 = false)
		{
			T[] source = smethod_6(type_0, bool_0);
			if (source.Any())
			{
				return source.First();
			}
			return null;
		}

		public static T smethod_5(PropertyInfo propertyInfo_0, bool bool_0 = false)
		{
			T[] source = smethod_7(propertyInfo_0, bool_0);
			if (source.Any())
			{
				return source.First();
			}
			return null;
		}

		public static T[] smethod_6(Type type_0, bool bool_0 = false)
		{
			return (T[])type_0.GetCustomAttributes(typeof(T), bool_0);
		}

		public static T[] smethod_7(PropertyInfo propertyInfo_0, bool bool_0 = false)
		{
			return (T[])propertyInfo_0.GetCustomAttributes(typeof(T), bool_0);
		}
	}
}
