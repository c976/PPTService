using System;

namespace Cvte.Paint.Framework
{
	public static class InterfaceCreator
	{
		private static readonly InnerInterfaceCreator InnerInterfaceCreator = new InnerInterfaceCreator();

		public static TInterface GetInterfaceInstance<TInterface>(params object[] args)
		{
			return InnerInterfaceCreator.GetInterfaceInstance<TInterface>(args);
		}

		public static void RegisterInterfaceInstance<TInterface>(Func<TInterface> func, bool onlyInstance = false)
		{
			InnerInterfaceCreator.RegisterInterfaceInstance(func, onlyInstance);
		}

		public static void RegisterInterfaceInstance<TInterface>(Func<object, TInterface> func, bool onlyInstance = false)
		{
			InnerInterfaceCreator.RegisterInterfaceInstance(func, onlyInstance);
		}

		public static void UnregisterInterfaceInstance<TInterface>()
		{
			InnerInterfaceCreator.UnregisterInterfaceInstance<TInterface>();
		}
	}
}
