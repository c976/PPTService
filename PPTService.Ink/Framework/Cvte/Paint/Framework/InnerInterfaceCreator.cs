using System;
using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	internal class InnerInterfaceCreator : IInterfaceCreator
	{
		private readonly Dictionary<Type, InterfaceInfo> _interfaceDictionary = new Dictionary<Type, InterfaceInfo>();

		public TInterface GetInterfaceInstance<TInterface>(params object[] args)
		{
			object[] array = null;
			object obj = null;
			bool flag = false;
			if (args != null)
			{
				if (args.Length > 1)
				{
					array = args;
					flag = true;
				}
				else if (args.Length == 1)
				{
					obj = args[0];
				}
			}
			Type typeFromHandle = typeof(TInterface);
			if (!_interfaceDictionary.ContainsKey(typeFromHandle))
			{
				return default(TInterface);
			}
			InterfaceInfo interfaceInfo = _interfaceDictionary[typeFromHandle];
			if (!interfaceInfo.SingleInstance)
			{
				return CreateInterfaceInstance<TInterface>(interfaceInfo.CreateInterfaceHandler, flag ? array : obj);
			}
			if (interfaceInfo.Instance != null)
			{
				return (TInterface)interfaceInfo.Instance;
			}
			interfaceInfo.Instance = CreateInterfaceInstance<TInterface>(interfaceInfo.CreateInterfaceHandler, flag ? array : obj);
			return (TInterface)interfaceInfo.Instance;
		}

		private TInterface CreateInterfaceInstance<TInterface>(Delegate handler, object args)
		{
			if ((object)handler == null)
			{
				throw new NotSupportedException("未将实例绑定到指定接口，请调用RegisterInterfaceInstance方法，以绑定接口实例");
			}
			Func<TInterface> func = handler as Func<TInterface>;
			if (func == null)
			{
				Func<object, TInterface> func2 = handler as Func<object, TInterface>;
				if (func2 != null)
				{
					return func2(args);
				}
				return default(TInterface);
			}
			return func();
		}

		public void RegisterInterfaceInstance<TInterface>(Func<TInterface> func, bool onlyInstance = false)
		{
			RegisterInterfaceInstanceFromDelegate<TInterface>(func, onlyInstance);
		}

		public void RegisterInterfaceInstance<TInterface>(Func<object, TInterface> func, bool onlyInstance = false)
		{
			RegisterInterfaceInstanceFromDelegate<TInterface>(func, onlyInstance);
		}

		private void RegisterInterfaceInstanceFromDelegate<TInterface>(Delegate func, bool onlyInstance = false)
		{
			Type typeFromHandle = typeof(TInterface);
			if (!_interfaceDictionary.ContainsKey(typeFromHandle))
			{
				_interfaceDictionary.Add(typeFromHandle, new InterfaceInfo
				{
					SingleInstance = onlyInstance,
					CreateInterfaceHandler = func
				});
				return;
			}
			throw new InvalidOperationException("该接口已被绑定，请先解除绑定后重试，参见：UnregisterInterfaceInstance方法");
		}

		public void UnregisterInterfaceInstance<TInterface>()
		{
			Type typeFromHandle = typeof(TInterface);
			_interfaceDictionary.Remove(typeFromHandle);
		}
	}
}
