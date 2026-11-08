using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Reflection
{
	public static class Reflector
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5_0
		{
			public string propertyName;

			internal PropertyInfo _003CSetProperty_003Eb__0(Type t)
			{
				return t.GetProperty(propertyName, _instanceBindFlags);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass15_0
		{
			public string methodName;

			public Type[] parameterTypes;

			internal MethodInfo _003CInvokeMethodInternal_003Eb__0(Type t)
			{
				return t.GetMethod(methodName, _instanceBindFlags, null, parameterTypes, null);
			}
		}

		[CompilerGenerated]
		private sealed class _003CGetAllTypesInHierarchy_003Ed__18 : IEnumerable<Type>, IEnumerator<Type>, IEnumerable, IDisposable, IEnumerator
		{
			private int _003C_003E1__state;

			private Type _003C_003E2__current;

			private int _003C_003El__initialThreadId;

			private Type type;

			public Type _003C_003E3__type;

			Type IEnumerator<Type>.Current
			{
				[DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			[DebuggerHidden]
			public _003CGetAllTypesInHierarchy_003Ed__18(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
				_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				int num = _003C_003E1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						goto IL_001b;
					}
					_003C_003E1__state = -1;
				}
				else
				{
					if (false)
					{
						goto IL_001b;
					}
					_003C_003E1__state = -1;
					if (4 != 0)
					{
						goto IL_0067;
					}
				}
				type = type.BaseType;
				goto IL_0067;
				IL_0067:
				int num2;
				if (-1 == 0 || type != null)
				{
					_003C_003E2__current = type;
					_003C_003E1__state = 1;
					if (0 == 0)
					{
						num2 = 1;
						goto IL_004d;
					}
				}
				return false;
				IL_004d:
				return (byte)num2 != 0;
				IL_001b:
				num2 = 0;
				if (num2 == 0)
				{
					return (byte)num2 != 0;
				}
				goto IL_004d;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator<Type> IEnumerable<Type>.GetEnumerator()
			{
				if (_003C_003E1__state != -2)
				{
					goto IL_0027;
				}
				goto IL_0044;
				IL_0044:
				if (_003C_003El__initialThreadId != Environment.CurrentManagedThreadId)
				{
					goto IL_0027;
				}
				_003C_003E1__state = 0;
				goto IL_0051;
				IL_002c:
				_003CGetAllTypesInHierarchy_003Ed__18 obj;
				if (4 != 0)
				{
					if (false)
					{
						goto IL_0044;
					}
					obj.type = _003C_003E3__type;
					return obj;
				}
				goto IL_0051;
				IL_0051:
				obj = this;
				if (false)
				{
					goto IL_0027;
				}
				goto IL_002c;
				IL_0027:
				obj = new _003CGetAllTypesInHierarchy_003Ed__18(0);
				goto IL_002c;
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return ((IEnumerable<Type>)this).GetEnumerator();
			}
		}

		private static BindingFlags _instanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static BindingFlags _staticBindFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		public static object GetField([NotNull] object instance, [NotNull] string fieldName)
		{
			if (8u != 0)
			{
			}
			string fieldName2;
			while (true)
			{
				fieldName2 = fieldName;
				if (instance == null)
				{
					if (-1 == 0)
					{
						continue;
					}
					throw new ArgumentNullException("instance");
				}
				break;
			}
			while (fieldName2 == null || -1 == 0)
			{
				if (-1 == 0)
				{
					continue;
				}
				throw new ArgumentNullException("fieldName");
			}
			FieldInfo member = GetMember(instance.GetType(), (Type t) => t.GetField(fieldName2, _instanceBindFlags));
			if (member == null)
			{
				throw new MissingFieldException(fieldName2);
			}
			return member.GetValue(instance);
		}

		public static void SetField([NotNull] object instance, [NotNull] string fieldName, object value)
		{
			while (true)
			{
				if (0 == 0)
				{
					if (true)
					{
					}
				}
				while (true)
				{
					string fieldName2 = fieldName;
					if (4 == 0)
					{
						break;
					}
					if (instance == null)
					{
						throw new ArgumentNullException("instance");
					}
					if (fieldName2 == null || 7 == 0)
					{
						throw new ArgumentNullException("fieldName");
					}
					if (3 == 0)
					{
						continue;
					}
					FieldInfo member = GetMember(instance.GetType(), (Type t) => t.GetField(fieldName2, _instanceBindFlags));
					if (member == null)
					{
						throw new MissingFieldException(fieldName2);
					}
					member.SetValue(instance, value);
					return;
				}
			}
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
			while (true)
			{
				if (0 == 0)
				{
					if (true)
					{
					}
				}
				while (true)
				{
					string propertyName2 = propertyName;
					if (4 == 0)
					{
						break;
					}
					if (instance == null)
					{
						throw new ArgumentNullException("instance");
					}
					do
					{
						if (propertyName2 == null)
						{
							throw new ArgumentNullException("propertyName");
						}
					}
					while (7 == 0);
					if (3 == 0)
					{
						continue;
					}
					PropertyInfo member = GetMember(instance.GetType(), (Type t) => t.GetProperty(propertyName2, _instanceBindFlags));
					if (member == null)
					{
						throw new MissingFieldException(propertyName2);
					}
					return member.GetValue(instance, null);
				}
			}
		}

		public static void SetProperty([NotNull] object instance, [NotNull] string propertyName, object value)
		{
			_003C_003Ec__DisplayClass5_0 obj = default(_003C_003Ec__DisplayClass5_0);
			if (0 == 0)
			{
				if (-1 == 0)
				{
					goto IL_009d;
				}
				obj = new _003C_003Ec__DisplayClass5_0();
			}
			goto IL_0091;
			IL_0091:
			obj.propertyName = propertyName;
			goto IL_009d;
			IL_009d:
			if (instance != null)
			{
				if (false)
				{
					goto IL_0091;
				}
				if (obj.propertyName == null)
				{
					throw new ArgumentNullException("propertyName");
				}
				if (6 == 0)
				{
					return;
				}
				if (2 != 0)
				{
					PropertyInfo member = GetMember(instance.GetType(), obj._003CSetProperty_003Eb__0);
					if (member == null)
					{
						throw new MissingFieldException(obj.propertyName);
					}
					member.SetValue(instance, value, null);
					return;
				}
			}
			throw new ArgumentNullException("instance");
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
			while (true)
			{
				if (false)
				{
					goto IL_0016;
				}
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				goto IL_0063;
				IL_0016:
				if (false)
				{
					continue;
				}
				if (0 == 0)
				{
					if (false)
					{
						goto IL_0063;
					}
					throw new ArgumentNullException("propertyName");
				}
				break;
				IL_0063:
				while (propertyName != null)
				{
					if (false)
					{
						continue;
					}
					PropertyInfo property = type.GetProperty(propertyName, _staticBindFlags);
					if (property == null)
					{
						throw new MissingFieldException(propertyName);
					}
					property.SetValue(null, value, null);
					return;
				}
				goto IL_0016;
			}
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
			while (true)
			{
				if (type == null)
				{
					if (6 != 0 && 0 == 0)
					{
						throw new ArgumentNullException("type");
					}
				}
				else if (5 != 0 && 0 == 0)
				{
					break;
				}
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
			if ((4 != 0 && type == null) ? true : false)
			{
				throw new ArgumentNullException("type");
			}
			return InvokeMethodInternal(null, type, methodName, parameters, parameterTypes);
		}

		[DebuggerStepThrough]
		private static object InvokeMethodInternal(object instance, Type type, string methodName, [NotNull] object[] parameters)
		{
			object[] parameters2;
			do
			{
				parameters2 = parameters;
			}
			while (false);
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			if (parameters2 == null)
			{
				throw new ArgumentNullException("parameters");
			}
			MethodInfo[] array = (from x in GetAllTypesInHierarchy(type).SelectMany((Type t) => t.GetMethods((instance == null) ? _staticBindFlags : _instanceBindFlags))
				where 8 != 0 && 5 != 0 && x.Name == methodName && 2 != 0 && CanMatch(x, parameters2)
				select x).ToArray();
			if (array.Length == 1)
			{
				return array.Single().Invoke(instance, parameters2);
			}
			if (array.Length == 0)
			{
				throw new MissingMethodException();
			}
			throw new AmbiguousMatchException();
		}

		public static object InvokeMethodInternal(object instance, [NotNull] Type type, [NotNull] string methodName, [NotNull] object[] parameters, [NotNull] Type[] parameterTypes)
		{
			_003C_003Ec__DisplayClass15_0 obj = default(_003C_003Ec__DisplayClass15_0);
			if (3 != 0)
			{
				obj = new _003C_003Ec__DisplayClass15_0();
				obj.methodName = methodName;
				obj.parameterTypes = parameterTypes;
				if (obj.methodName == null)
				{
					throw new ArgumentNullException("methodName");
				}
				if (parameters == null)
				{
					throw new ArgumentNullException("parameters");
				}
			}
			MethodInfo methodInfo;
			while (true)
			{
				if (instance == null)
				{
					while (7 != 0)
					{
						methodInfo = type.GetMethod(obj.methodName, _staticBindFlags, null, obj.parameterTypes, null);
						if (8 != 0)
						{
							goto end_IL_0054;
						}
					}
					continue;
				}
				methodInfo = GetMember(instance.GetType(), obj._003CInvokeMethodInternal_003Eb__0);
				break;
				continue;
				end_IL_0054:
				break;
			}
			return methodInfo.Invoke(instance, parameters);
		}

		private static bool CanMatch(MethodInfo method, object[] parameters)
		{
			object obj2 = default(object);
			while (true)
			{
				ParameterInfo[] parameters2;
				int num;
				if (0 == 0)
				{
					parameters2 = method.GetParameters();
					num = parameters2.Length;
					while (num == parameters.Length)
					{
						num = 0;
						if (num == 0)
						{
							if (num != 0)
							{
								continue;
							}
							goto IL_002b;
						}
						goto IL_006b;
					}
					goto IL_0022;
				}
				goto IL_0034;
				IL_006a:
				num = 0;
				goto IL_006b;
				IL_006b:
				return (byte)num != 0;
				IL_002b:
				int num2;
				if (uint.MaxValue != 0)
				{
					num2 = num;
				}
				goto IL_0070;
				IL_0070:
				if (false)
				{
					continue;
				}
				if (num2 >= parameters2.Length)
				{
					break;
				}
				goto IL_0034;
				IL_0034:
				if (false)
				{
					goto IL_0022;
				}
				object obj = parameters[num2];
				if (0 == 0)
				{
					obj2 = obj;
				}
				if (false)
				{
					goto IL_006a;
				}
				ParameterInfo parameterInfo = parameters2[num2];
				if (obj2 == null)
				{
					if (!parameterInfo.ParameterType.IsClass)
					{
						return false;
					}
				}
				else if (!parameterInfo.ParameterType.IsInstanceOfType(obj2))
				{
					goto IL_006a;
				}
				num2++;
				goto IL_0070;
				IL_0022:
				return false;
			}
			return true;
		}

		private static T GetMember<T>(Type type, Func<Type, T> func) where T : MemberInfo
		{
			T val = default(T);
			while (true)
			{
				if (2 != 0)
				{
					val = null;
				}
				while (true)
				{
					if (type != null)
					{
						if (false)
						{
							continue;
						}
						val = func(type);
						if (!(val != null))
						{
							if (7 == 0 || 3 == 0)
							{
								break;
							}
							Type baseType = type.BaseType;
							if (0 == 0)
							{
								type = baseType;
							}
							continue;
						}
					}
					return val;
				}
			}
		}

		[IteratorStateMachine(typeof(_003CGetAllTypesInHierarchy_003Ed__18))]
		private static IEnumerable<Type> GetAllTypesInHierarchy(Type type)
		{
			//yield-return decompiler failed: Unable to find 'return true' for yield return
			return new _003CGetAllTypesInHierarchy_003Ed__18(-2)
			{
				_003C_003E3__type = type
			};
		}
	}
}
