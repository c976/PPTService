using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.Linq
{
	
	
	public static class EnumerableExtensions
	{
		public static void ForEach< T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (T item in source)
			{
				action(item);
			}
		}

		public static async Task ForEachAsync< T>(this IEnumerable<T> source, Func<T, Task> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (T item in source)
			{
				await action(item);
			}
		}

		public static void ForEach< T>(this IEnumerable<T> source, Action<T, int> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			int num = 0;
			foreach (T item in source)
			{
				action(item, num++);
			}
		}

		public static async Task ForEachAsync< T>(this IEnumerable<T> source, Func<T, int, Task> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			int itemIndex = 0;
			foreach (T item in source)
			{
				await action(item, itemIndex++);
			}
		}

		public static int FirstIndexOf< T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			int num = -1;
			foreach (T item in source)
			{
				num++;
				if (predicate(item))
				{
					return num;
				}
			}
			return -1;
		}

		public static int LastIndexOf< T>(this IEnumerable<T> source, Predicate<T> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (predicate == null)
			{
				throw new ArgumentNullException("predicate");
			}
			ICollection<T> obj = (source as ICollection<T>) ?? source.ToArray();
			int num = obj.Count;
			foreach (T item in obj.Reverse())
			{
				num--;
				if (predicate(item))
				{
					return num;
				}
			}
			return -1;
		}
	}
}
