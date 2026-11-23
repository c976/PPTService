using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public abstract class NodeParserBase< T> : IStorableNodeParser
	{
		public bool CanParse(Type type)
		{
			return typeof(T).IsAssignableFrom(type);
		}

		public bool CanDeparse(object value)
		{
			return value is T;
		}

		public object Parse(StorableNode node, Type type, PropertyInfo property)
		{
			return DoParse(node, type, property);
		}

		public void Deparse(string name, object value, StorableNode node, PropertyInfo property)
		{
			DoDeparse(name, (T)value, node, property);
		}

		protected abstract T DoParse(StorableNode node, Type type, PropertyInfo property);

		protected abstract void DoDeparse(string name, T value, StorableNode node, PropertyInfo property);
	}
}
