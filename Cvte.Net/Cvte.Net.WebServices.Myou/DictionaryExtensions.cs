using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cvte.Net.WebServices.Myou
{
	
	
	internal static class DictionaryExtensions
	{
		public static IDictionary<string, object> AddRange(this IDictionary<string, object> target, IReadOnlyDictionary<string, object> dictionary)
		{
			foreach (KeyValuePair<string, object> item in dictionary.Where( (KeyValuePair<string, object> pair) => !target.ContainsKey(pair.Key)))
			{
				target.Add(item.Key, item.Value);
			}
			return target;
		}

		public static IDictionary<string, object> AddRange(this IDictionary<string, object> target, IDictionary<string, object> dictionary)
		{
			foreach (KeyValuePair<string, object> item in dictionary.Where( (KeyValuePair<string, object> pair) => !target.ContainsKey(pair.Key)))
			{
				target.Add(item.Key, item.Value);
			}
			return target;
		}

		public static IDictionary<string, object> Clone(this IDictionary<string, object> target)
		{
			return target.ToDictionary( (KeyValuePair<string, object> p) => p.Key,  (KeyValuePair<string, object> p) => p.Value, StringComparer.Ordinal);
		}
	}
}
