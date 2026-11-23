using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations.Concurrent
{
	
	
	
	[DebuggerDisplay("{Value,nq} [{State}] // {LastUpdateTime}")]
	internal struct ProcessSafeValueEntry< TValue>
	{
		public DateTimeOffset LastUpdateTime { get; }

		public TValue ExternalValue { get; }

		public TValue Value { get; }

		public ProcessSafeValueState State { get; }

		internal ProcessSafeValueEntry(TValue value, TValue externalValue, DateTimeOffset timestamp, ProcessSafeValueState state)
		{
			Value = value;
			ExternalValue = externalValue;
			LastUpdateTime = timestamp;
			State = state;
		}
	}
}
