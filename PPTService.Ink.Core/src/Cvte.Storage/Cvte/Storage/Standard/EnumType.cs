using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public class EnumType : IStorableType
	{
		public string Value { get; set; }

		public StorableNode Deparse()
		{
			return new StorableNode
			{
				Value = Value
			};
		}

		public void Parse(StorableNode node)
		{
			Value = node.StringValue;
		}
	}
}
