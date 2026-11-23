using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations.Concurrent
{
	
	
	
	public struct TimedKeyValues< TKey,  TValue> : IEquatable<TimedKeyValues<TKey, TValue>>
	{
		public IReadOnlyDictionary<TKey, TValue> KeyValues { get; }

		public DateTimeOffset Time { get; }

		public TimedKeyValues(IReadOnlyDictionary<TKey, TValue> keyValues, DateTimeOffset time)
		{
			if (keyValues == null)
			{
				throw new ArgumentNullException("keyValues");
			}
			KeyValues = keyValues;
			Time = time;
		}

		
		public override bool Equals(object obj)
		{
			if (obj is TimedKeyValues<TKey, TValue>)
			{
				TimedKeyValues<TKey, TValue> other = (TimedKeyValues<TKey, TValue>)obj;
				return Equals(other);
			}
			return false;
		}

		public bool Equals( TimedKeyValues<TKey, TValue> other)
		{
			if (EqualityComparer<IReadOnlyDictionary<TKey, TValue>>.Default.Equals(KeyValues, other.KeyValues))
			{
				return Time.Equals(other.Time);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (unchecked(-746944874 * -1521134295) + EqualityComparer<IReadOnlyDictionary<TKey, TValue>>.Default.GetHashCode(KeyValues)) * -1521134295 + Time.GetHashCode();
		}

		public static bool operator ==( TimedKeyValues<TKey, TValue> left,  TimedKeyValues<TKey, TValue> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=( TimedKeyValues<TKey, TValue> left,  TimedKeyValues<TKey, TValue> right)
		{
			return !(left == right);
		}
	}
}
