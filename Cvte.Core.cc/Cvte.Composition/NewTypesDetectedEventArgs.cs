using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cvte.Composition
{
	public class NewTypesDetectedEventArgs : EventArgs
	{
		private readonly ITypeCollection _collection;

		public Assembly TargetAssembly { get; private set; }

		internal NewTypesDetectedEventArgs(ITypeCollection typeCollection, Assembly targetAssembly)
		{
			_collection = typeCollection;
			TargetAssembly = targetAssembly;
		}

		public IEnumerable<T> GetTypeOf<T>()
		{
			return _collection.OfType<T>((Type t) => true);
		}

		public IEnumerable<T> GetTypeOf<T>(Predicate<Type> typeFilter)
		{
			return _collection.OfType<T>(typeFilter);
		}
	}
}
