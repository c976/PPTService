using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public abstract class EncodedType : IStorableType
	{
		public StorableNode Deparse()
		{
			return new StorableNode
			{
				Value = Encode()
			};
		}

		public void Parse(StorableNode node)
		{
			TryDecode(node.StringValue);
		}

		public abstract string Encode();

		public abstract bool TryDecode(string value);
	}
}
