using System;
using System.Collections;
using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	public class FixedQueue<T> : ICollection, IEnumerable, IEnumerable<T>
	{
		private readonly Queue<T> _innerQueue = new Queue<T>();

		public int MaxCount { get; private set; }

		public int Count => _innerQueue.Count;

		public object SyncRoot => ((ICollection)_innerQueue).SyncRoot;

		public bool IsSynchronized => false;

		public FixedQueue(int maxCount)
		{
			MaxCount = maxCount;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _innerQueue.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _innerQueue.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_innerQueue).CopyTo(array, index);
		}

		public void CopyTo(T[] array, int index)
		{
			_innerQueue.CopyTo(array, index);
		}

		public void Enqueue(T item)
		{
			if (_innerQueue.Count > MaxCount)
			{
				throw new InvalidOperationException("集合中的元素已超过最大限定值。");
			}
			if (_innerQueue.Count == MaxCount)
			{
				_innerQueue.Dequeue();
			}
			_innerQueue.Enqueue(item);
		}

		[Obsolete("请使用不带参数的 Dequeue 方法代替，这个方法传入的参数没有被使用")]
		public T Dequeue(T item)
		{
			return _innerQueue.Dequeue();
		}

		public T Dequeue()
		{
			return _innerQueue.Dequeue();
		}

		public T Peek()
		{
			return _innerQueue.Peek();
		}

		public bool Contains(T item)
		{
			return _innerQueue.Contains(item);
		}

		public void Clear()
		{
			_innerQueue.Clear();
		}
	}
}
