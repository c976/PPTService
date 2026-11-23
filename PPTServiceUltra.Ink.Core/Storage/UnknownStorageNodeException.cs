using System;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Storage.AdvancedSaveInfos;
using PPTServiceUltra.Ink.Core.Storage.Exceptions;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class UnknownStorageNodeException : UnknowStorageNodeException
	{
		public StorableNode Node { get; }

		
		
		public AdvancedSaveInfoMetadata SaveInfoMetadata
		{
			
			get;
			
			set;
		}

		public UnknownStorageNodeException(Type type, StorableNode node)
			: base(type)
		{
			Node = node;
		}

		public UnknownStorageNodeException(Type type, string typeName, StorableNode node)
			: base(type, typeName)
		{
			Node = node;
		}

		public UnknownStorageNodeException(string typeName, StorableNode node)
			: base(typeName)
		{
			Node = node;
		}
	}
}
