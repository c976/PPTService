using System;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.WebServices.Exceptions
{
	
	
	public class ParseException : Exception
	{
		public ParseException()
		{
		}

		public ParseException(string message)
			: base(message)
		{
		}

		public ParseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
