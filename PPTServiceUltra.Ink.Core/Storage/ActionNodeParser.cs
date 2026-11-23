using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class ActionNodeParser : StorableNodeParserBase<ActionType>
	{
		protected override ActionType DoParse(StorableNode node, Type type, PropertyInfo property)
		{
			ActionType actionType;
			if (!TypeFactory.TryCreate(ActionType.DecodeName(node.StringValue), out actionType))
			{
				throw new UnknownStorageNodeException(type, node);
			}
			actionType.Parse(node);
			return actionType;
		}

		protected override void DoDeparse(string name, ActionType value, StorableNode node, PropertyInfo property)
		{
			string value2 = value.Encode();
			node.AddChild(new StorableNode(name)
			{
				Value = value2,
				IsDeltaSupported = (property == null || property.GetMemberIsDeltaSupported())
			});
		}
	}
}
