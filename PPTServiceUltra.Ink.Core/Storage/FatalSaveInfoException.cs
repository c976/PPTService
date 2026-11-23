using System;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class FatalSaveInfoException : StorageException
	{
		public override string Message { get; }

		public FatalSaveInfoException(Exception innerException)
			: base(innerException.Message, innerException)
		{
			Message = innerException.Message;
			Info = innerException.Message;
		}
	}
}
