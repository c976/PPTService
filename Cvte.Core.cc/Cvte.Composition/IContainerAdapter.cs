using System;
using System.Collections.Generic;

namespace Cvte.Composition
{
	public interface IContainerAdapter
	{
		object Get(Type type);

		object Get(Type type, string key);

		IEnumerable<object> GetAll(Type type);

		T Get<T>();

		T Get<T>(string key);

		IEnumerable<T> GetAll<T>();

		void Set<T>(T value);

		void Set<T>(string key, T value);
	}
}
