using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Reflection
{
	public static class Reflector
	{
		private static BindingFlags _instanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static BindingFlags _staticBindFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		public static object GetField([NotNull] object instance, [NotNull] string fieldName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo member = GetMember(instance.GetType(), (Type t) => t.GetField(fieldName, _instanceBindFlags));
			if (member == null)
			{
				throw new MissingFieldException(fieldName);
			}
			return member.GetValue(instance);
		}

		public static void SetField([NotNull] object instance, [NotNull] string fieldName, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo member = GetMember(instance.GetType(), (Type t) => t.GetField(fieldName, _instanceBindFlags));
			if (member == null)
			{
				throw new MissingFieldException(fieldName);
			}
			member.SetValue(instance, value);
		}

		public static object GetStaticField([NotNull] Type type, [NotNull] string fieldName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo field = type.GetField(fieldName, _staticBindFlags);
			if (field == null)
			{
				throw new MissingFieldException(fieldName);
			}
			return field.GetValue(null);
		}

		public static void SetStaticField([NotNull] Type type, [NotNull] string fieldName, object value)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo field = type.GetField(fieldName, _staticBindFlags);
			if (field == null)
			{
				throw new MissingFieldException(fieldName);
			}
			field.SetValue(null, value);
		}

		public static object GetProperty([NotNull] object instance, [NotNull] string propertyName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo member = GetMember(instance.GetType(), (Type t) => t.GetProperty(propertyName, _instanceBindFlags));
			if (member == null)
			{
				throw new MissingFieldException(propertyName);
			}
			return member.GetValue(instance, null);
		}

		public static void SetProperty([NotNull] object instance, [NotNull] string propertyName, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo member = GetMember(instance.GetType(), (Type t) => t.GetProperty(propertyName, _instanceBindFlags));
			if (member == null)
			{
				throw new MissingFieldException(propertyName);
			}
			member.SetValue(instance, value, null);
		}

		public static object GetStaticProperty([NotNull] Type type, [NotNull] string propertyName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo property = type.GetProperty(propertyName, _staticBindFlags);
			if (property == null)
			{
				throw new MissingFieldException(propertyName);
			}
			return property.GetValue(null, null);
		}

		public static void SetStaticProperty([NotNull] Type type, [NotNull] string propertyName, object value)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			PropertyInfo property = type.GetProperty(propertyName, _staticBindFlags);
			if (property == null)
			{
				throw new MissingFieldException(propertyName);
			}
			property.SetValue(null, value, null);
		}

		public static object InvokeMethod([NotNull] object instance, [NotNull] string methodName, [NotNull] object[] parameters)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return InvokeMethodInternal(instance, instance.GetType(), methodName, parameters);
		}

		public static object InvokeMethod([NotNull] object instance, [NotNull] string methodName)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return InvokeMethodInternal(instance, instance.GetType(), methodName, new object[0], new Type[0]);
		}

		public static object InvokeMethod([NotNull] object instance, [NotNull] string methodName, [NotNull] object[] parameters, [NotNull] Type[] parameterTypes)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return InvokeMethodInternal(instance, instance.GetType(), methodName, parameters, parameterTypes);
		}

		public static object InvokeStaticMethod([NotNull] Type type, [NotNull] string methodName, [NotNull] object[] parameters)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return InvokeMethodInternal(null, type, methodName, parameters);
		}

		public static object InvokeStaticMethod([NotNull] Type type, [NotNull] string methodName)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return InvokeMethodInternal(null, type, methodName, new object[0], new Type[0]);
		}

		public static object InvokeStaticMethod([NotNull] Type type, [NotNull] string methodName, [NotNull] object[] parameters, [NotNull] Type[] parameterTypes)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return InvokeMethodInternal(null, type, methodName, parameters, parameterTypes);
		}

		[DebuggerStepThrough]
		private static object InvokeMethodInternal(object instance, Type type, string methodName, [NotNull] object[] parameters)
		{
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			MethodInfo[] array = (from x in GetAllTypesInHierarchy(type).SelectMany((Type t) => t.GetMethods((instance == null) ? _staticBindFlags : _instanceBindFlags))
				where x.Name == methodName && CanMatch(x, parameters)
				select x).ToArray();
			if (array.Length != 1)
			{
				if (array.Length == 0)
				{
					throw new MissingMethodException();
				}
				throw new AmbiguousMatchException();
			}
			return array.Single().Invoke(instance, parameters);
		}

		public static object InvokeMethodInternal(object instance, [NotNull] Type type, [NotNull] string methodName, [NotNull] object[] parameters, [NotNull] Type[] parameterTypes)
		{
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			MethodInfo methodInfo = ((instance != null) ? GetMember(instance.GetType(), (Type t) => t.GetMethod(methodName, _instanceBindFlags, null, parameterTypes, null)) : type.GetMethod(methodName, _staticBindFlags, null, parameterTypes, null));
			return methodInfo.Invoke(instance, parameters);
		}

		private static bool CanMatch(MethodInfo method, object[] parameters)
		{
			ParameterInfo[] parameters2 = method.GetParameters();
			if (parameters2.Length != parameters.Length)
			{
				return false;
			}
			int num = 0;
			while (true)
			{
				if (num < parameters2.Length)
				{
					object obj = parameters[num];
					ParameterInfo parameterInfo = parameters2[num];
					if (obj == null)
					{
						if (!parameterInfo.ParameterType.IsClass)
						{
							return false;
						}
					}
					else if (!parameterInfo.ParameterType.IsInstanceOfType(obj))
					{
						break;
					}
					num++;
					continue;
				}
				return true;
			}
			return false;
		}

		private static T GetMember<T>(Type type, Func<Type, T> func) where T : MemberInfo
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
