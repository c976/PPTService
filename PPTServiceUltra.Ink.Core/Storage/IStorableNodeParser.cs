using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	internal interface IStorableNodeParser
	{
		bool CanParse(Type type);

		bool CanDeparse(object value);

		object Parse(StorableNode node, Type type, PropertyInfo property);

		void Deparse(string name, object value, StorableNode node, PropertyInfo property);
	}
}
