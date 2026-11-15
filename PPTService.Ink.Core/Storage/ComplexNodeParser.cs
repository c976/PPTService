using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage.Standard;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class ComplexNodeParser : StorableNodeParserBase<ComplexType>
	{
		private readonly Func<IReferenceManager> _getReferenceManager;

		public ComplexNodeParser(Func<IReferenceManager> getReferenceManager)
		{
			_getReferenceManager = getReferenceManager;
		}

		protected override ComplexType DoParse(StorableNode node, Type type, PropertyInfo property)
		{
			StorableNode firstNode = node.FirstNode;
			string name = firstNode.Name;
			ComplexType complexType;
			if (!TypeFactory.TryCreate(type, name, out complexType))
			{
				throw new UnknownStorageNodeException(type, name, node);
			}
			complexType.SetReferenceManager(_getReferenceManager());
			complexType.Parse(firstNode);
			return complexType;
		}

		protected override void DoDeparse(string name, ComplexType value, StorableNode node, PropertyInfo property)
		{
			value.SetReferenceManager(_getReferenceManager());
			StorableNode node2 = value.Deparse();
			StorableNode storableNode = new StorableNode
			{
				Name = name,
				IsDeltaSupported = (property == null || property.GetMemberIsDeltaSupported())
			};
			storableNode.AddChild(node2);
			node.AddChild(storableNode);
		}
	}
}
