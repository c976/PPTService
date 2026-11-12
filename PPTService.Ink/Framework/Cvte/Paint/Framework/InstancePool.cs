using System;
using System.Collections.Generic;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Framework
{
	public class InstancePool<T> where T : class
	{
		[NotNull]
		private readonly Dictionary<Type, List<WeakReference<T>>> _poolDictionary;

		[CanBeNull]
		private readonly Predicate<T> _filter;

		public InstancePool()
		{
			_poolDictionary = new Dictionary<Type, List<WeakReference<T>>>();
		}

		public InstancePool([NotNull] Predicate<T> filter)
			: this()
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			_filter = filter;
		}

		[NotNull]
		public TActual Get<TActual>() where TActual : class, T, new()
		{
			List<WeakReference<T>> value;
			if (!_poolDictionary.TryGetValue(typeof(TActual), out value))
			{
				value = new List<WeakReference<T>>();
				TActual val = CreateInstance<TActual>();
				value.Add(new WeakReference<T>((T)val));
				_poolDictionary.Add(typeof(TActual), value);
				return val;
			}
			return GetFromPool<TActual>(value);
		}

		[NotNull]
		private TActual GetFromPool<TActual>([NotNull] List<WeakReference<T>> list) where TActual : class, T, new()
		{
			TActual val = null;
			List<WeakReference<T>> list2 = new List<WeakReference<T>>();
			foreach (WeakReference<T> item in list)
			{
				T target;
				bool flag = item.TryGetTarget(out target);
				if (val == null && flag && Filter(target))
				{
					val = (TActual)target;
				}
				if (!flag)
				{
					list2.Add(item);
				}
			}
			list.RemoveAll(list2.Contains);
			if (val == null)
			{
				val = CreateInstance<TActual>();
				list.Add(new WeakReference<T>((T)val));
			}
			return val;
		}

		[NotNull]
		private TActual CreateInstance<TActual>() where TActual : class, T, new()
		{
			TActual val = new TActual();
			if (!Filter((T)val))
			{
				throw new InvalidOperationException("实例池中新创建的实例被认为不满足获取实例的条件，这可能是实例是否可用的判定条件过于苛刻。如果依然决定继续获取实例，可能导致无限创建实例并最终导致资源耗尽。请检查传入构造函数中的 filter 参数是否被正确实现。");
			}
			return val;
		}

		private bool Filter([NotNull] T instance)
		{
			if (_filter != null)
			{
				return _filter(instance);
			}
			return true;
		}
	}
}
