using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class UnknowStorageException : StorageException
	{
		public override string Message => Content;

		public string Content { get; set; }

		public UnknowStorageException()
		{
		}

		public UnknowStorageException(string message, Exception innerException)
			: base(message, innerException)
		{
			Content = message;
		}
	}
}
