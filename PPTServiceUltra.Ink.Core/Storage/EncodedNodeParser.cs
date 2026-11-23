using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class EncodedNodeParser : StorableNodeParserBase<EncodedType>
	{
		protected override EncodedType DoParse(StorableNode node, Type type, PropertyInfo property)
		{
			EncodedType storableType;
			if (!TypeFactory.TryCreate<EncodedType>(type, out storableType))
			{
				throw new UnknownStorageNodeException(type, node);
			}
			if (!storableType.TryDecode(node.StringValue))
			{
				throw new StorageNoteFormatException(type, node.StringValue);
			}
			return storableType;
		}

		protected override void DoDeparse(string name, EncodedType value, StorableNode node, PropertyInfo property)
		{
			string value2 = value.Encode();
			if (!string.IsNullOrEmpty(value2))
			{
				node.AddChild(new StorableNode(name)
				{
					Value = value2,
					IsDeltaSupported = (property == null || property.GetMemberIsDeltaSupported())
				});
			}
		}
	}
}
