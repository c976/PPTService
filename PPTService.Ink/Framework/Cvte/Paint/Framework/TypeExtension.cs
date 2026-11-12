using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cvte.Storage;

namespace Cvte.Paint.Framework
{
	public static class TypeExtension
	{
		public static T Create<T>(this Type type)
		{
			return Expression.Lambda<Func<T>>(Expression.New(type), new ParameterExpression[0]).Compile()();
		}

		public static bool TryGetInterfaceGenericType(this Type type, Type interfaceType, out Type genericType)
		{
			List<Type> list = (from i in type.GetInterfaces()
				where i.Name.Equals(interfaceType.Name)
				select i).ToList();
			if (!list.Any())
			{
				genericType = null;
				return false;
			}
			if (list.Count == 1)
			{
				genericType = list.First().GenericTypeArguments.FirstOrDefault();
				return true;
			}
			Type baseType = type.BaseType;
			while (baseType != null)
			{
				foreach (Type item in (from i in baseType.GetInterfaces()
					where i.Name.Equals(interfaceType.Name)
					select i).ToList())
				{
					list.Remove(item);
				}
				if (list.Count <= 1)
				{
					break;
				}
				baseType = type.BaseType;
			}
			if (list.Count == 1)
			{
				genericType = list.First().GenericTypeArguments.FirstOrDefault();
				return true;
			}
			genericType = null;
			return false;
		}

		public static bool TryGetSaveInfoType(this Type type, Type interfaceType, out Type saveInfoType)
		{
			if (!typeof(ISavable).IsAssignableFrom(type))
			{
				saveInfoType = null;
				return false;
			}
			return type.TryGetInterfaceGenericType(interfaceType, out saveInfoType);
		}
	}
}
