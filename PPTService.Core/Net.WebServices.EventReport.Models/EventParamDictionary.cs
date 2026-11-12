using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Core.Net.WebServices.EventReport.Models
{
	
	
	public class EventParamDictionary : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		
		
		private List<KeyValuePair<string, object>> InnerList
		{
			
			get;
		}

		public int Count => InnerList.Count;

		public bool IsReadOnly => true;

		public object this[string key]
		{
			get
			{
				object value;
				if (TryGetValue(key, out value))
				{
					return value;
				}
				return null;
			}
			set
			{
				Remove(key);
				Add(key, value);
			}
		}

		public ICollection<string> Keys => InnerList.Select( (KeyValuePair<string, object> t) => t.Key).ToList();

		public ICollection<object> Values => InnerList.Select( (KeyValuePair<string, object> t) => t.Value).ToList();

		public EventParamDictionary(int capacity = 0)
		{
			InnerList = new List<KeyValuePair<string, object>>(capacity);
		}

		public EventParamDictionary( params KeyValuePair<string, object>[] eventParams)
		{
			InnerList = new List<KeyValuePair<string, object>>(eventParams);
		}

		public EventParamDictionary( params ValueTuple<string, object>[] eventParams)
		{
			InnerList = new List<KeyValuePair<string, object>>(eventParams.Length);
			InnerList.AddRange(eventParams.Select( ( ValueTuple<string, object> t) => new KeyValuePair<string, object>(t.Item1, t.Item2)));
		}

		
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return InnerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)InnerList).GetEnumerator();
		}

		public void Add( KeyValuePair<string, object> item)
		{
			InnerList.Add(item);
		}

		public void Clear()
		{
			InnerList.Clear();
		}

		public bool Contains( KeyValuePair<string, object> item)
		{
			return InnerList.Contains(item);
		}

		public void CopyTo( KeyValuePair<string, object>[] array, int arrayIndex)
		{
			InnerList.CopyTo(array, arrayIndex);
		}

		public bool Remove( KeyValuePair<string, object> item)
		{
			return InnerList.Remove(item);
		}

		public bool ContainsKey(string key)
		{
			return InnerList.Any( (KeyValuePair<string, object> t) => t.Key == key);
		}

		public void Add(string key, object value)
		{
			Add(new KeyValuePair<string, object>(key, value));
		}

		public bool Remove(string key)
		{
			return InnerList.RemoveAll(( KeyValuePair<string, object> t) => t.Key == key) > 0;
		}

		public bool TryGetValue(string key, out object value)
		{
			value = InnerList.FirstOrDefault( (KeyValuePair<string, object> t) => t.Key == key).Value;
			return value != null;
		}
	}
}
