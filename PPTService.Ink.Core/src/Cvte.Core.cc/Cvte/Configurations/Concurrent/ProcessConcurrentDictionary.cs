using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using PPTService.Ink.Core.Configurations.Utils;

namespace PPTService.Ink.Core.Configurations.Concurrent
{
	internal class ProcessConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		private readonly ConcurrentDictionary<TKey, ProcessSafeValueEntry<TValue>> _keyValues = new ConcurrentDictionary<TKey, ProcessSafeValueEntry<TValue>>();

		public ICollection<TKey> Keys => _keyValues.Keys;

		public TValue this[TKey key]
		{
			get
			{
				ProcessSafeValueEntry<TValue> value;
				if (!_keyValues.TryGetValue(key, out value))
				{
					return default(TValue);
				}
				return value.Value;
			}
			set
			{
				_keyValues.AddOrUpdate(key, (TKey _) => CreateInternalValue(value), (TKey _, ProcessSafeValueEntry<TValue> existed) => CreateInternalValue(value, existed));
			}
		}

		ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keyValues.Keys;

		ICollection<TValue> IDictionary<TKey, TValue>.Values => _keyValues.Values.Select((ProcessSafeValueEntry<TValue> x) => x.Value).ToList();

		int ICollection<KeyValuePair<TKey, TValue>>.Count => _keyValues.Count;

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

		public void UpdateValuesFromExternal(FileInfo file, Action<ICriticalReadWriteContext<TKey, TValue>> duringCriticalReadWriteContext)
		{
			string fullPath = Path.GetFullPath(file.FullName);
			string text = (OSUtils.IsPathCaseSensitive() ? fullPath : fullPath.ToUpperInvariant());
			OSUtils.IsPathCaseSensitive();
			text = text.Replace("/", "_").Replace("\\", "_");
			UpdateValuesFromExternal(text, duringCriticalReadWriteContext);
		}

		public void UpdateValuesFromExternal(string name, Action<ICriticalReadWriteContext<TKey, TValue>> duringCriticalReadWriteContext)
		{
			using (Mutex mutex = new Mutex(false, name))
			{
				try
				{
					mutex.WaitOne();
				}
				catch (AbandonedMutexException)
				{
				}
				try
				{
					CriticalReadWriteContext<TKey, TValue> obj = new CriticalReadWriteContext<TKey, TValue>(UpdateValuesFromExternalCore);
					duringCriticalReadWriteContext(obj);
				}
				finally
				{
					mutex.ReleaseMutex();
				}
			}
		}

		public bool ContainsKey(TKey key)
		{
			return _keyValues.ContainsKey(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			ProcessSafeValueEntry<TValue> value2;
			if (_keyValues.TryGetValue(key, out value2))
			{
				value = value2.Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		public bool TryRemove(TKey key, out TValue value)
		{
			value = _keyValues.AddOrUpdate(key, (TKey _) => CreateInternalDeletedValue(), (TKey _, ProcessSafeValueEntry<TValue> existed) => CreateInternalDeletedValue(existed)).ExternalValue;
			return true;
		}

		private TimedKeyValues<TKey, TValue> UpdateValuesFromExternalCore(IReadOnlyDictionary<TKey, TValue> externalKeyValues, DateTimeOffset externalUpdateTime)
		{
			IEnumerable<TKey> enumerable = _keyValues.Keys.Except(externalKeyValues.Keys);
			foreach (KeyValuePair<TKey, TValue> external in externalKeyValues)
			{
				_keyValues.AddOrUpdate(external.Key, (TKey _) => CreateExternalValue(external.Value, externalUpdateTime), (TKey _, ProcessSafeValueEntry<TValue> existed) => CreateExternalValue(external.Value, existed, externalUpdateTime));
			}
			foreach (TKey item in enumerable)
			{
				_keyValues.AddOrUpdate(item, (TKey _) => CreateExternalDeletedValue(externalUpdateTime), (TKey _, ProcessSafeValueEntry<TValue> existed) => CreateExternalLatestOrDeletedValue(existed, externalUpdateTime));
			}
			List<KeyValuePair<TKey, ProcessSafeValueEntry<TValue>>> list = _keyValues.Where((KeyValuePair<TKey, ProcessSafeValueEntry<TValue>> x) => x.Value.State != ProcessSafeValueState.Deleted).ToList();
			return new TimedKeyValues<TKey, TValue>(list.ToDictionary((KeyValuePair<TKey, ProcessSafeValueEntry<TValue>> x) => x.Key, (KeyValuePair<TKey, ProcessSafeValueEntry<TValue>> x) => x.Value.Value), (list.Count == 0) ? externalUpdateTime : list.Max((KeyValuePair<TKey, ProcessSafeValueEntry<TValue>> x) => x.Value.LastUpdateTime));
		}

		private static ProcessSafeValueEntry<TValue> CreateInternalValue(TValue value)
		{
			return new ProcessSafeValueEntry<TValue>(value, default(TValue), DateTimeOffset.UtcNow, ProcessSafeValueState.Changed);
		}

		private static ProcessSafeValueEntry<TValue> CreateInternalValue(TValue value, ProcessSafeValueEntry<TValue> existedEntry)
		{
			if (!object.Equals(value, existedEntry.Value))
			{
				return new ProcessSafeValueEntry<TValue>(value, existedEntry.ExternalValue, DateTimeOffset.UtcNow, ProcessSafeValueState.Changed);
			}
			return existedEntry;
		}

		private static ProcessSafeValueEntry<TValue> CreateInternalDeletedValue()
		{
			return new ProcessSafeValueEntry<TValue>(default(TValue), default(TValue), DateTimeOffset.UtcNow, ProcessSafeValueState.Deleted);
		}

		private static ProcessSafeValueEntry<TValue> CreateInternalDeletedValue(ProcessSafeValueEntry<TValue> existedEntry)
		{
			return new ProcessSafeValueEntry<TValue>(existedEntry.ExternalValue, default(TValue), DateTimeOffset.UtcNow, ProcessSafeValueState.Deleted);
		}

		private static ProcessSafeValueEntry<TValue> CreateExternalValue(TValue value, DateTimeOffset externalUpdateTime)
		{
			return new ProcessSafeValueEntry<TValue>(value, value, externalUpdateTime, ProcessSafeValueState.NotChanged);
		}

		private static ProcessSafeValueEntry<TValue> CreateExternalValue(TValue value, ProcessSafeValueEntry<TValue> existedEntry, DateTimeOffset externalUpdateTime)
		{
			if (object.Equals(value, existedEntry.Value))
			{
				return existedEntry;
			}
			if (existedEntry.LastUpdateTime > externalUpdateTime)
			{
				return new ProcessSafeValueEntry<TValue>(existedEntry.Value, existedEntry.Value, existedEntry.LastUpdateTime, ProcessSafeValueState.NotChanged);
			}
			return new ProcessSafeValueEntry<TValue>(value, value, externalUpdateTime, ProcessSafeValueState.NotChanged);
		}

		private static ProcessSafeValueEntry<TValue> CreateExternalDeletedValue(DateTimeOffset deletedTime)
		{
			return new ProcessSafeValueEntry<TValue>(default(TValue), default(TValue), deletedTime, ProcessSafeValueState.Deleted);
		}

		private static ProcessSafeValueEntry<TValue> CreateExternalLatestOrDeletedValue(ProcessSafeValueEntry<TValue> existedEntry, DateTimeOffset externalUpdateTime)
		{
			if (existedEntry.State == ProcessSafeValueState.Changed)
			{
				return new ProcessSafeValueEntry<TValue>(existedEntry.Value, existedEntry.Value, existedEntry.LastUpdateTime, ProcessSafeValueState.NotChanged);
			}
			return new ProcessSafeValueEntry<TValue>(default(TValue), default(TValue), externalUpdateTime, ProcessSafeValueState.Deleted);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			ProcessSafeValueEntry<TValue> value;
			if (_keyValues.TryGetValue(item.Key, out value))
			{
				return object.Equals(value, item);
			}
			return false;
		}

		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		{
			ProcessSafeValueEntry<TValue> value2;
			if (_keyValues.TryGetValue(key, out value2))
			{
				value = value2.Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			_keyValues.TryAdd(key, CreateInternalValue(value));
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			_keyValues.TryAdd(item.Key, CreateInternalValue(item.Value));
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			ProcessSafeValueEntry<TValue> value;
			return _keyValues.TryRemove(key, out value);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			ProcessSafeValueEntry<TValue> value;
			return _keyValues.TryRemove(item.Key, out value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			_keyValues.Clear();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", "序号必须大于等于 0。");
			}
			_keyValues.ToArray().CopyTo(array, arrayIndex);
		}

		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			foreach (KeyValuePair<TKey, ProcessSafeValueEntry<TValue>> keyValue in _keyValues)
			{
				yield return new KeyValuePair<TKey, TValue>(keyValue.Key, keyValue.Value.Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();
		}
	}
}
