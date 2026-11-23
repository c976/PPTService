using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal static class TypeFactory
	{
		private static readonly IDictionary<Type, IDictionary<string, Type>> ComplexMapping;

		private static readonly IDictionary<Type, Type> StorableTypeMapping;

		private static readonly IDictionary<string, Type> ActionTypeMapping;

		static TypeFactory()
		{
			ComplexMapping = new Dictionary<Type, IDictionary<string, Type>>();
			StorableTypeMapping = new Dictionary<Type, Type>();
			ActionTypeMapping = new Dictionary<string, Type>();
			LoadType();
		}

		public static void LoadType()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type typeFromHandle = typeof(ComplexType);
			Type typeFromHandle2 = typeof(ActionType);
			Type typeFromHandle3 = typeof(IStorableType);
			Type[] exportedTypes = executingAssembly.GetExportedTypes();
			foreach (Type type in exportedTypes)
			{
				if (typeFromHandle.IsAssignableFrom(type))
				{
					ComplexTypeAttribute customAttribute = type.GetCustomAttribute<ComplexTypeAttribute>(false);
					if (customAttribute == null)
					{
						continue;
					}
					Type belong = customAttribute.Belong;
					string key = customAttribute.Key;
					IDictionary<string, Type> value;
					if (ComplexMapping.TryGetValue(belong, out value))
					{
						value.Add(key, type);
					}
					else
					{
						ComplexMapping.Add(belong, new Dictionary<string, Type> { { key, type } });
					}
				}
				if (typeFromHandle2.IsAssignableFrom(type))
				{
					ActionTypeAttribute customAttribute2 = type.GetCustomAttribute<ActionTypeAttribute>(false);
					if (customAttribute2 == null)
					{
						continue;
					}
					string key2 = customAttribute2.Key;
					if (!ActionTypeMapping.ContainsKey(key2))
					{
						ActionTypeMapping.Add(key2, type);
					}
				}
				if (typeFromHandle3.IsAssignableFrom(type) && !type.IsAbstract)
				{
					Type key3 = type;
					if (!StorableTypeMapping.ContainsKey(key3))
					{
						StorableTypeMapping.Add(key3, type);
					}
				}
			}
		}

		public static bool TryCreate(Type belong, string key, out ComplexType complexType)
		{
			IDictionary<string, Type> value;
			Type value2;
			if (ComplexMapping.TryGetValue(belong, out value) && value.TryGetValue(key, out value2))
			{
				complexType = (ComplexType)Activator.CreateInstance(value2);
				return true;
			}
			complexType = null;
			return false;
		}

		public static bool TryCreate< T>(Type key, out T storableType) where T : IStorableType
		{
			Type value;
			if (StorableTypeMapping.TryGetValue(key, out value))
			{
				storableType = (T)Activator.CreateInstance(value);
				return true;
			}
			storableType = default(T);
			return false;
		}

		public static bool TryCreate(string key, out ActionType actionType)
		{
			Type value;
			if (ActionTypeMapping.TryGetValue(key, out value))
			{
				actionType = Activator.CreateInstance(value) as ActionType;
				return true;
			}
			actionType = null;
			return false;
		}
	}
}
