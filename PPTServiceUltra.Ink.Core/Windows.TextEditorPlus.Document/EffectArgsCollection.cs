using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class EffectArgsCollection : IList<ITextEffectArgs>, ICollection<ITextEffectArgs>, IEnumerable<ITextEffectArgs>, IEnumerable
	{
		private readonly List<ITextEffectArgs> _data = new List<ITextEffectArgs>();

		public int Count => _data.Count;

		public bool IsReadOnly => false;

		public ITextEffectArgs this[int index]
		{
			get
			{
				return _data[index];
			}
			set
			{
				_data[index] = value;
			}
		}

		public IEnumerator<ITextEffectArgs> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(ITextEffectArgs item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			Type type = item.GetType();
			if (_data.Any((ITextEffectArgs x) => x.GetType() == type))
			{
				throw new ArgumentOutOfRangeException("item", "same effect is exist");
			}
			_data.Add(item);
		}

		public void Clear()
		{
			_data.Clear();
		}

		public bool Contains(ITextEffectArgs item)
		{
			return _data.Contains(item);
		}

		public bool ContainsType(Type type)
		{
			return _data.Any((ITextEffectArgs x) => x.GetType() == type);
		}

		public void CopyTo(ITextEffectArgs[] array, int arrayIndex)
		{
			_data.CopyTo(array, arrayIndex);
		}

		public bool Remove(ITextEffectArgs item)
		{
			return _data.Remove(item);
		}

		public int IndexOf(ITextEffectArgs item)
		{
			return _data.IndexOf(item);
		}

		public void Insert(int index, ITextEffectArgs item)
		{
			_data.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			_data.RemoveAt(index);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			EffectArgsCollection effectArgsCollection = obj as EffectArgsCollection;
			if (effectArgsCollection != null && effectArgsCollection._data.Count == 0 && _data.Count == 0)
			{
				return true;
			}
			if (effectArgsCollection?._data.Count != _data.Count)
			{
				return false;
			}
			return _data.SequenceEqual(effectArgsCollection._data);
		}

		public override int GetHashCode()
		{
			if (_data.Count > 0)
			{
				return _data.Aggregate(0, (int v, ITextEffectArgs e) => v + e.GetHashCode());
			}
			return 0;
		}

		public EffectArgsCollection Clone()
		{
			EffectArgsCollection effectArgsCollection = new EffectArgsCollection();
			foreach (ITextEffectArgs datum in _data)
			{
				effectArgsCollection.Add(datum.Clone());
			}
			return effectArgsCollection;
		}
	}
}
