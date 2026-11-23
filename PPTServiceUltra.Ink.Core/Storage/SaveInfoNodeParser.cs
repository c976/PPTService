using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class SaveInfoNodeParser : NodeParserBase<SaveInfo>
	{
		private readonly Func<StorableNode, SaveInfo> _parseFunc;

		private readonly Func<SaveInfo, StorableNode> _deparseFunc;

		public SaveInfoNodeParser(Func<StorableNode, SaveInfo> parseFunc, Func<SaveInfo, StorableNode> deparseFunc)
		{
			_parseFunc = parseFunc;
			_deparseFunc = deparseFunc;
		}

		protected override SaveInfo DoParse(StorableNode node, Type type, PropertyInfo property)
		{
			if (property.IsDefined(typeof(ComplexSaveInfoAttribute)))
			{
				if (node.FirstNode == null)
				{
					throw new StorageNoteFormatException(type, node.StringValue);
				}
				return _parseFunc(node.FirstNode);
			}
			SaveInfoContractAttribute customAttribute = type.GetCustomAttribute<SaveInfoContractAttribute>();
			if (customAttribute == null)
			{
				throw new StorageNodeFormatException(type, node.StringValue);
			}
			node.InternalKey = node.Name;
			node.ConsistentName = node.Name;
			node.Name = customAttribute.Name;
			return _parseFunc(node);
		}

		protected override void DoDeparse(string name, SaveInfo value, StorableNode node, PropertyInfo property)
		{
			StorableNode storableNode = _deparseFunc(value);
			if (property.IsDefined(typeof(ComplexSaveInfoAttribute)))
			{
				StorableNode storableNode2 = new StorableNode
				{
					Name = name
				};
				storableNode2.AddChild(storableNode);
				storableNode2.IsDeltaSupported = property == null || property.GetMemberIsDeltaSupported();
				node.AddChild(storableNode2);
			}
			else
			{
				storableNode.Name = name;
				storableNode.IsDeltaSupported = property == null || property.GetMemberIsDeltaSupported();
				node.AddChild(storableNode);
			}
		}
	}
}
