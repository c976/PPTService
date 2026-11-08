using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Cvte.Net
{
	
	
	public static class AttributeUtility< SA_T0> where SA_T0 : Attribute
	{
		public static bool IsDefined(object obj, bool inherit = false)
		{
			return IsDefined(obj.GetType());
		}

		public static bool IsDefined(Type type, bool inherit = false)
		{
			return type.IsDefined(typeof(SA_T0), inherit);
		}

		public static bool IsDefined(PropertyInfo property, bool inherit = false)
		{
			return property.IsDefined(typeof(SA_T0), inherit);
		}

		public static SA_T0 GetFirstCustomAttribute(object obj, bool inherit = false)
		{
			return GetFirstCustomAttribute(obj.GetType());
		}

		public static SA_T0 GetFirstCustomAttribute(Type type, bool inherit = false)
		{
			SA_T0[] customAttributes = GetCustomAttributes(type, inherit);
			if (customAttributes.Any())
			{
				return customAttributes.First();
			}
			return null;
		}

		public static SA_T0 GetFirstCustomAttribute(PropertyInfo property, bool inherit = false)
		{
			SA_T0[] customAttributes = GetCustomAttributes(property, inherit);
			if (customAttributes.Any())
			{
				return customAttributes.First();
			}
			return null;
		}

		public static SA_T0[] GetCustomAttributes(Type type, bool inherit = false)
		{
			return (SA_T0[])type.GetCustomAttributes(typeof(SA_T0), inherit);
		}

		public static SA_T0[] GetCustomAttributes(PropertyInfo property, bool inherit = false)
		{
			return (SA_T0[])property.GetCustomAttributes(typeof(SA_T0), inherit);
		}
	}
}
