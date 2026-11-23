using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class FileContentParseException : StorageException
	{
		public FileContentParseException(string message)
			: base(message)
		{
		}

		public FileContentParseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
