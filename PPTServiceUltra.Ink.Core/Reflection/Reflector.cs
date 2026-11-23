using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Reflection
{
	
	
	public static class Reflector
	{
		private const BindingFlags InstanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private const BindingFlags StaticBindFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		public static object GetField(object instance, string fieldName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo member = GetMember(instance.GetType(), (Type t) => t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			if (member == null)
			{
				throw new MissingFieldException(fieldName);
			}
			return member.GetValue(instance);
		}

		public static void SetField(object instance, string fieldName, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo member = GetMember(instance.GetType(), (Type t) => t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			if (member == null)
			{
				throw new MissingFieldException(fieldName);
			}
			member.SetValue(instance, value);
		}

		public static object GetStaticField(Type type, string fieldName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new MissingFieldException(fieldName);
			}
			return field.GetValue(null);
		}

		public static void SetStaticField(Type type, string fieldName, object value)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo field = type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new MissingFieldException(fieldName);
			}
			field.SetValue(null, value);
		}

		public static object GetProperty(object instance, string propertyName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo member = GetMember(instance.GetType(), (Type t) => t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			if (member == null)
			{
				throw new MissingFieldException(propertyName);
			}
			return member.GetValue(instance, null);
		}

		public static Func<object, object> GetPropertyGetAccessor(Type type, string propertyName)
		{
			if ((object)type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				throw new MissingFieldException(propertyName);
			}
			MethodInfo getMethod = property.GetGetMethod(true);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "o");
			return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Call(Expression.Convert(parameterExpression, getMethod.DeclaringType), getMethod), typeof(object)), new ParameterExpression[1] { parameterExpression }).Compile();
		}

		public static void SetProperty(object instance, string propertyName, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo member = GetMember(instance.GetType(), (Type t) => t.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			if (member == null)
			{
				throw new MissingFieldException(propertyName);
			}
			member.SetValue(instance, value, null);
		}

		public static object GetStaticProperty(Type type, string propertyName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				throw new MissingFieldException(propertyName);
			}
			return property.GetValue(null, null);
		}

		public static void SetStaticProperty(Type type, string propertyName, object value)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (property == null)
			{
				throw new MissingFieldException(propertyName);
			}
			property.SetValue(null, value, null);
		}

		public static object InvokeMethod(object instance, string methodName, object[] parameters)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return InvokeMethodInternal(instance, instance.GetType(), methodName, parameters);
		}

		public static object InvokeMethod(object instance, string methodName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return InvokeMethodInternal(instance, instance.GetType(), methodName, new object[0], new Type[0]);
		}

		public static object InvokeMethod(object instance, string methodName, object[] parameters, Type[] parameterTypes)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return InvokeMethodInternal(instance, instance.GetType(), methodName, parameters, parameterTypes);
		}

		public static object InvokeStaticMethod(Type type, string methodName, object[] parameters)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return InvokeMethodInternal(null, type, methodName, parameters);
		}

		public static object InvokeStaticMethod(Type type, string methodName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return InvokeMethodInternal(null, type, methodName, new object[0], new Type[0]);
		}

		public static object InvokeStaticMethod(Type type, string methodName, object[] parameters, Type[] parameterTypes)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return InvokeMethodInternal(null, type, methodName, parameters, parameterTypes);
		}

		[DebuggerStepThrough]
		private static object InvokeMethodInternal(object instance, Type type, string methodName, object[] parameters)
		{
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			MethodInfo[] array = (from x in GetAllTypesInHierarchy(type).SelectMany( (Type t) => t.GetMethods((instance == null) ? (BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) : (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)))
				where x.Name == methodName && CanMatch(x, parameters)
				select x).ToArray();
			if (array.Length == 1)
			{
				return array.Single().Invoke(instance, parameters);
			}
			if (array.Length == 0)
			{
				throw new MissingMethodException();
			}
			throw new AmbiguousMatchException();
		}

		[Obsolete("这个方法只是私有方法，请调用 InvokeMethod")]
		public static object InvokeMethodInternal(object instance, Type type, string methodName, object[] parameters, Type[] parameterTypes)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			MethodInfo methodInfo = ((instance != null) ? GetMember(instance.GetType(), (Type t) => t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null)) : type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null));
			return methodInfo.Invoke(instance, parameters);
		}

		private static bool CanMatch(MethodInfo method, object[] parameters)
		{
			ParameterInfo[] parameters2 = method.GetParameters();
			if (parameters2.Length != parameters.Length)
			{
				return false;
			}
			for (int i = 0; i < parameters2.Length; i++)
			{
				object obj = parameters[i];
				ParameterInfo parameterInfo = parameters2[i];
				if (obj == null)
				{
					if (!parameterInfo.ParameterType.IsClass)
					{
						return false;
					}
				}
				else if (!parameterInfo.ParameterType.IsInstanceOfType(obj))
				{
					return false;
				}
			}
			return true;
		}

		private static T GetMember< T>(Type type, Func<Type, T> func) where T : MemberInfo
		{
			T val = null;
			while (type != null)
			{
				val = func(type);
				if (val != null)
				{
					break;
				}
				type = type.BaseType;
			}
			return val;
		}

		private static IEnumerable<Type> GetAllTypesInHierarchy(Type type)
		{
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}
	}
}
