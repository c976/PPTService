using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.Standard;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class CompoundNodeParser : StorableNodeParserBase<CompoundType>
	{
		private readonly Func<IReferenceManager> _getReferenceManager;

		public CompoundNodeParser(Func<IReferenceManager> getReferenceManager)
		{
			_getReferenceManager = getReferenceManager;
		}

		protected override CompoundType DoParse(StorableNode node, Type type, PropertyInfo property)
		{
			CompoundType compoundType = (CompoundType)Activator.CreateInstance(type);
			compoundType.Parse(node);
			return compoundType;
		}

		protected override void DoDeparse(string name, CompoundType value, StorableNode node, PropertyInfo property)
		{
			value.Name = name;
			value.SetReferenceManager(_getReferenceManager());
			StorableNode storableNode = value.Deparse();
			storableNode.IsDeltaSupported = property == null || property.GetMemberIsDeltaSupported();
			node.AddChild(storableNode);
		}
	}
}
