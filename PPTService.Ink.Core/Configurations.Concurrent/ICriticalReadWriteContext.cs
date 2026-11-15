using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Configurations.Concurrent
{
	
	public interface ICriticalReadWriteContext<TKey, TValue>
	{
		
		
		TimedKeyValues<TKey, TValue> MergeExternalKeyValues(IReadOnlyDictionary<TKey, TValue> keyValues, DateTimeOffset? externalUpdateTime = null);
	}
}
