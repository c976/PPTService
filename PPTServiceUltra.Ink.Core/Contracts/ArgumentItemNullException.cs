using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PPTServiceUltra.Ink.Core.Contracts
{
	[Serializable]
	
	
	public class ArgumentItemNullException : ArgumentException
	{
		public ArgumentItemNullException()
			: base("集合参数中有一项或多项为 null。")
		{
		}

		public ArgumentItemNullException(string paramName)
			: base("集合参数中有一项或多项为 null，参数名是 " + paramName + "。", paramName)
		{
		}

		public ArgumentItemNullException(string paramName, string message)
			: base(message, paramName)
		{
		}

		public ArgumentItemNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ArgumentItemNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
