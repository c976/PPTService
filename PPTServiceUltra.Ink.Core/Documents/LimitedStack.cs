using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Documents
{
	
	
	public class LimitedStack< T> : IEnumerable<T>, IEnumerable, ICollection, IReadOnlyCollection<T>
	{
		private readonly LinkedList<T> _linkedList = new LinkedList<T>();

		public uint LimitedCount { get; set; } = uint.MaxValue;

		public int Count => ((ICollection)_linkedList).Count;

		public bool IsSynchronized => ((ICollection)_linkedList).IsSynchronized;

		public object SyncRoot => ((ICollection)_linkedList).SyncRoot;

		public void Clear()
		{
			_linkedList.Clear();
		}

		public T Peek()
		{
			return _linkedList.Last.Value;
		}

		public T Pop()
		{
			if (_linkedList.Count == 0)
			{
				throw new InvalidOperationException("栈为空，不能弹出值");
			}
			T value = _linkedList.Last.Value;
			_linkedList.RemoveLast();
			return value;
		}

		public void Push(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			_linkedList.AddLast(item);
			if (Count > LimitedCount * 2)
			{
				while (Count > LimitedCount)
				{
					_linkedList.RemoveFirst();
				}
			}
		}

		public bool Contains(T item)
		{
			return _linkedList.Contains(item);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_linkedList).CopyTo(array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)_linkedList).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<T>)_linkedList).GetEnumerator();
		}
	}
}
