using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PPTService.Ink.Core.Contracts
{
	[Serializable]
	
	
	public class ReturnNullException : ReturnValueException
	{
		public ReturnNullException()
			: base("成员的返回值为 null，但 null 返回值不符合契约约束。")
		{
		}

		public ReturnNullException(string memberName)
			: base("成员的返回值为 null，但 null 返回值不符合契约约束。成员名称是：" + memberName + "。")
		{
		}

		public ReturnNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public ReturnNullException(string memberName, string message)
			: base(message, memberName)
		{
		}

		protected ReturnNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
