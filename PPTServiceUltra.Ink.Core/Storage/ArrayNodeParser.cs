using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class ArrayNodeParser : StorableNodeParserBase<ArrayType>
	{
		protected override ArrayType DoParse(StorableNode node, Type type, PropertyInfo property)
		{
			if (!node.HasChild)
			{
				return null;
			}
			ArrayType arrayType = (ArrayType)Activator.CreateInstance(type);
			arrayType.Parse(node);
			return arrayType;
		}

		protected override void DoDeparse(string name, ArrayType value, StorableNode node, PropertyInfo property)
		{
			StorableNode storableNode = value.Deparse();
			storableNode.Name = name;
			storableNode.IsArray = true;
			storableNode.IsDeltaSupported = property == null || property.GetMemberIsDeltaSupported();
			node.AddChild(storableNode);
		}
	}
}
