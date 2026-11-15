using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PPTService.Ink.Core.Contracts
{
	[Serializable]
	
	
	public class ContractException : Exception
	{
		public ContractException()
			: base("没有按照契约来使用相关的应用程序编程接口。")
		{
		}

		public ContractException(string message)
			: base(message)
		{
		}

		public ContractException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected ContractException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
