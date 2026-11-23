using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations.Concurrent
{
	
	
	internal class CriticalReadWriteContext<TKey, TValue> : ICriticalReadWriteContext<TKey, TValue>
	{
		
		private readonly Func<IReadOnlyDictionary<TKey, TValue>, DateTimeOffset, TimedKeyValues<TKey, TValue>> _keyValueMergingFunc;

		public CriticalReadWriteContext( Func<IReadOnlyDictionary<TKey, TValue>, DateTimeOffset, TimedKeyValues<TKey, TValue>> keyValueMergingFunc)
		{
			if (keyValueMergingFunc == null)
			{
				throw new ArgumentNullException("keyValueMergingFunc");
			}
			_keyValueMergingFunc = keyValueMergingFunc;
		}

		
		
		public TimedKeyValues<TKey, TValue> MergeExternalKeyValues(IReadOnlyDictionary<TKey, TValue> keyValues, DateTimeOffset? externalUpdateTime = null)
		{
			return _keyValueMergingFunc(keyValues, externalUpdateTime ?? DateTimeOffset.UtcNow);
		}
	}
}
