using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Cli.Core
{
	
	
	internal class ListGroup<T> : IEnumerable<KeyValuePair<int, string>>, IEnumerable, IEnumerable<KeyValuePair<string, T>> where T : class,  IReadOnlyList<string>
	{
		private readonly List<string> _keys = new List<string>();

		
		private readonly List<T> _values = new List<T>();

		public void Add(string key,  T value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			_keys.Add(key);
			_values.Add(value);
		}

		public void ReplaceKey(int index, string newKey)
		{
			if (newKey == null)
			{
				throw new ArgumentNullException("newKey");
			}
			_keys[index] = newKey;
		}

		
		public IEnumerable<KeyValuePair<int, string>> ForIndexes()
		{
			return this;
		}

		
		public IEnumerable<KeyValuePair<string, T>> ForOptions()
		{
			return this;
		}

		
		IEnumerator<KeyValuePair<int, string>> IEnumerable<KeyValuePair<int, string>>.GetEnumerator()
		{
			for (int i = 0; i < _keys.Count; i++)
			{
				yield return new KeyValuePair<int, string>(i, _keys[i]);
			}
		}

		
		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			for (int i = 0; i < _keys.Count; i++)
			{
				yield return new KeyValuePair<string, T>(_keys[i], _values[i]);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
