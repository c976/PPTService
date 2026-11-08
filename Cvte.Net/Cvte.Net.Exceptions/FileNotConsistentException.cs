using System;
using System.Runtime.CompilerServices;

namespace Cvte.Net.Exceptions
{
	
	
	public class FileNotConsistentException : Exception
	{
		public FileNotConsistentException(string message, Exception exception)
			: base(message, exception)
		{
		}

		public FileNotConsistentException(string message)
			: base(message)
		{
		}
	}
}
