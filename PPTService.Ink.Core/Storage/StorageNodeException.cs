using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class StorageNodeException : StorageException
	{
		protected StorageNodeException()
		{
		}

		public StorageNodeException(string message)
			: base(message)
		{
		}

		public StorageNodeException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
