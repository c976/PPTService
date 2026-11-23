using System;

namespace PPTServiceUltra.Ink.Core.Windows.Media
{
	public class FloodFillRangeQueue
	{
		private FloodFillRange[] _array;

		private int _size;

		private int _head;

		public int Count => _size;

		public FloodFillRange First => _array[_head];

		public FloodFillRangeQueue()
			: this(10000)
		{
		}

		public FloodFillRangeQueue(int initialSize)
		{
			_array = new FloodFillRange[initialSize];
			_head = 0;
			_size = 0;
		}

		public void Enqueue(ref FloodFillRange r)
		{
			if (_size + _head == _array.Length)
			{
				FloodFillRange[] array = new FloodFillRange[2 * _array.Length];
				Array.Copy(_array, _head, array, 0, _size);
				_array = array;
				_head = 0;
			}
			_array[_head + _size++] = r;
		}

		public FloodFillRange Dequeue()
		{
			FloodFillRange result = default(FloodFillRange);
			if (_size > 0)
			{
				result = _array[_head];
				_array[_head] = default(FloodFillRange);
				_head++;
				_size--;
			}
			return result;
		}
	}
}
