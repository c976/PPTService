using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Cli.Core
{
	
	
	[DebuggerDisplay("SingleOptimizedList {_firstValue,nq}, {_restValues}")]
	internal class SingleOptimizedList< T> : IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly T _firstValue;

		
		private List<T> _restValues;

		public int Count => (_restValues?.Count ?? 0) + 1;

		public T this[int index]
		{
			get
			{
				if (index != 0)
				{
					return _restValues[index - 1];
				}
				return _firstValue;
			}
		}

		public SingleOptimizedList(T firstValue)
		{
			_firstValue = firstValue;
		}

		public void Add(T value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (_restValues == null)
			{
				_restValues = new List<T>(1) { value };
			}
			else
			{
				_restValues.Add(value);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			yield return _firstValue;
			if (_restValues == null)
			{
				yield break;
			}
			foreach (T restValue in _restValues)
			{
				yield return restValue;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
