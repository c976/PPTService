using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class EnumNodeParser : StorableNodeParserBase<EnumType>
	{
		protected override EnumType DoParse(StorableNode node, Type type, PropertyInfo property)
		{
			EnumType storableType;
			if (!TypeFactory.TryCreate<EnumType>(type, out storableType))
			{
				throw new UnknownStorageNodeException(type, node);
			}
			storableType.Parse(node);
			return storableType;
		}

		protected override void DoDeparse(string name, EnumType value, StorableNode node, PropertyInfo property)
		{
			StorableNode storableNode = value.Deparse();
			node.AddChild(new StorableNode(name)
			{
				Value = storableNode.Value,
				IsDeltaSupported = (property == null || property.GetMemberIsDeltaSupported())
			});
		}
	}
}
