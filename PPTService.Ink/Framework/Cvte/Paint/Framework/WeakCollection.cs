using System;
using System.Collections.Generic;

namespace Cvte.Paint.Framework
{
	internal class WeakCollection<T> where T : class
	{
		private readonly List<WeakReference<T>> _weakList = new List<WeakReference<T>>();

		public void Add(T item)
		{
			for (int i = 0; i < _weakList.Count; i++)
			{
				T target;
				if (!_weakList[i].TryGetTarget(out target))
				{
					_weakList.RemoveAt(i);
					i--;
				}
			}
			_weakList.Add(new WeakReference<T>(item));
		}

		public bool Remove(T item)
		{
			for (int i = 0; i < _weakList.Count; i++)
			{
				T target;
				if (_weakList[i].TryGetTarget(out target))
				{
					if (object.Equals(target, item))
					{
						_weakList.RemoveAt(i);
						return true;
					}
				}
				else
				{
					_weakList.RemoveAt(i);
					i--;
				}
			}
			return false;
		}

		public void Clear()
		{
			_weakList.Clear();
		}

		public T[] TryGetItems(Func<T, bool> filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			List<T> list = new List<T>();
			for (int i = 0; i < _weakList.Count; i++)
			{
				T target;
				if (_weakList[i].TryGetTarget(out target))
				{
					if (filter(target))
					{
						list.Add(target);
					}
				}
				else
				{
					_weakList.RemoveAt(i);
					i--;
				}
			}
			return list.ToArray();
		}
	}
}
