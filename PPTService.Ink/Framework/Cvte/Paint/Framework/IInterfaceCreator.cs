using System;

namespace Cvte.Paint.Framework
{
	public interface IInterfaceCreator
	{
		TInterface GetInterfaceInstance<TInterface>(params object[] args);

		void RegisterInterfaceInstance<TInterface>(Func<TInterface> func, bool onlyInstance = false);

		void RegisterInterfaceInstance<TInterface>(Func<object, TInterface> func, bool onlyInstance = false);

		void UnregisterInterfaceInstance<TInterface>();
	}
}
