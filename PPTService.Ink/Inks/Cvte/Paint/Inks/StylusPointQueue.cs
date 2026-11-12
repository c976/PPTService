using System;
using System.Windows.Input;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks
{
	internal class StylusPointQueue
	{
		private readonly StylusPoint[] _array;

		private int _head;

		private int _tail;

		private int _size;

		private int _version;

		private readonly int _arrayLength;

		public int Count => _size;

		public StylusPoint this[int n]
		{
			get
			{
				return _array[(_head + n) % _arrayLength];
			}
			set
			{
				_array[(_head + n) % _arrayLength] = value;
			}
		}

		public StylusPointQueue(int capacity)
		{
			_array = new StylusPoint[capacity];
			_head = 0;
			_tail = 0;
			_size = 0;
			_arrayLength = capacity;
		}

		public void Enqueue(StylusPoint item)
		{
			_array[_tail] = item;
			_tail = (_tail + 1) % _arrayLength;
			_size++;
			_version++;
		}

		public StylusPoint Dequeue()
		{
			StylusPoint result = _array[_head];
			_head = (_head + 1) % _arrayLength;
			_size--;
			_version++;
			return result;
		}

		public void CopyToStylusPointCollection([NotNull] StylusPointCollection strokeTipCollection)
		{
			if (strokeTipCollection == null)
			{
				throw new ArgumentNullException("strokeTipCollection");
			}
			strokeTipCollection.Clear();
			for (int i = 0; i < _size; i++)
			{
				int num = (_head + i) % _arrayLength;
				strokeTipCollection.Add(_array[num]);
			}
		}
	}
}
