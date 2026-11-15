using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class FileContentDeparseException : StorageException
	{
		public FileContentDeparseException(string message)
			: base(message)
		{
		}

		public FileContentDeparseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
