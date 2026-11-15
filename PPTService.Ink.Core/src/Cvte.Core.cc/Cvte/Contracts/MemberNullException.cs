using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PPTService.Ink.Core.Contracts
{
	[Serializable]
	
	
	public class MemberNullException : InstanceNotReadyException
	{
		public string MemberName { get; }

		public MemberNullException()
			: base("因为类型成员尚未初始化（为 null），此方法目前不可被调用。")
		{
		}

		public MemberNullException(string memberName)
			: base("因为类型成员尚未初始化（为 null），此方法目前不可被调用。成员名称是：" + memberName + "。")
		{
			MemberName = memberName;
		}

		public MemberNullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public MemberNullException(string memberName, string message)
			: base(message)
		{
			MemberName = memberName;
		}

		protected MemberNullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
