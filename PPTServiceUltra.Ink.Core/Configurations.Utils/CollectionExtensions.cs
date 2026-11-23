using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations.Utils
{
	internal static class CollectionExtensions
	{
		
		internal static bool SequenceEqualsIgnoringOrder< T>(this ICollection<T> list1, ICollection<T> list2)
		{
			if (list1 == null)
			{
				throw new ArgumentNullException("list1");
			}
			if (list2 == null)
			{
				throw new ArgumentNullException("list2");
			}
			if (list1.Count == list2.Count)
			{
				return list1.All(list2.Contains);
			}
			return false;
		}
	}
}
