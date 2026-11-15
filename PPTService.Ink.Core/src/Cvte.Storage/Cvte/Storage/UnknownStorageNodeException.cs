using System;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage.AdvancedSaveInfos;
using PPTService.Ink.Core.Storage.Exception_;

namespace PPTService.Ink.Core.Storage
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
