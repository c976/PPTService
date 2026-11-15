using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PPTService.Ink.Core.Contracts
{
	[Serializable]
	
	
	public class ReturnValueException : ContractException
	{
		public string MemberName { get; }

		public ReturnValueException()
			: base("在获得成员的返回值时，返回值不符合契约约束。")
		{
		}

		public ReturnValueException(string message)
			: base(message)
		{
		}

		public ReturnValueException(string message, string memberName)
			: base(message)
		{
			MemberName = memberName;
		}

		public ReturnValueException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ReturnValueException(string message, string memberName, Exception innerException)
			: base(message, innerException)
		{
			MemberName = memberName;
		}

		protected ReturnValueException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
