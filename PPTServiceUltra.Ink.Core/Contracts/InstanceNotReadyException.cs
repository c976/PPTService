using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace PPTServiceUltra.Ink.Core.Contracts
{
	[Serializable]
	
	
	public class InstanceNotReadyException : ContractException
	{
		public InstanceNotReadyException()
		{
		}

		public InstanceNotReadyException(string message)
			: base(message)
		{
		}

		public InstanceNotReadyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected InstanceNotReadyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
