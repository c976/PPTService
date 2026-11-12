using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Cvte.Paint.Framework
{
	internal class NotificationVisualCollection : IList<Visual>, ICollection<Visual>, IEnumerable<Visual>, IEnumerable
	{
		private readonly VisualCollection _children;

		private readonly EventHandler _onChanged;

		public Visual this[int index]
		{
			get
			{
				return _children[index];
			}
			set
			{
				_children[index] = value;
				_onChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public int Count => _children.Count;

		public bool IsReadOnly => false;

		public NotificationVisualCollection(Visual parent, EventHandler onChanged)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			_children = new VisualCollection(parent);
			_onChanged = onChanged;
		}

		public void Add(Visual item)
		{
			_children.Add(item);
			_onChanged?.Invoke(this, EventArgs.Empty);
		}

		public void Clear()
		{
			_children.Clear();
			_onChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool Contains(Visual item)
		{
			return _children.Contains(item);
		}

		public void CopyTo(Visual[] array, int arrayIndex)
		{
			_children.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Visual> GetEnumerator()
		{
			foreach (Visual child in _children)
			{
				yield return child;
			}
		}

		public int IndexOf(Visual item)
		{
			return _children.IndexOf(item);
		}

		public void Insert(int index, Visual item)
		{
			_children.Insert(index, item);
			_onChanged?.Invoke(this, EventArgs.Empty);
		}

		public bool Remove(Visual item)
		{
			if (!_children.Contains(item))
			{
				return false;
			}
			_children.Remove(item);
			_onChanged?.Invoke(this, EventArgs.Empty);
			return true;
		}

		public void RemoveAt(int index)
		{
			_children.RemoveAt(index);
			_onChanged?.Invoke(this, EventArgs.Empty);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _children.GetEnumerator();
		}
	}
}
