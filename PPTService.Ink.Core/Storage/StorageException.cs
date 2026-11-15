using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class StorageException : Exception
	{
		public string Info { get; set; }

		public override string Message
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(base.Message))
				{
					return base.Message;
				}
				return Info;
			}
		}

		public StorageException()
		{
		}

		public StorageException(string message)
			: base(message)
		{
		}

		public StorageException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
