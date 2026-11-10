using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Cvte.Windows.Localization
{
	[DebuggerDisplay("Default={Default.Culture}, Current={Current.Culture}, Count={Count}")]
	public class LanguageCollection : ICollection<Language>, IEnumerable<Language>, IEnumerable
	{
		private readonly List<Language> _innerList = new List<Language>();

		public Language this[string name]
		{
			get
			{
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				return _innerList.FirstOrDefault((Language x) => x.Culture.Name == name);
			}
		}

		public Language this[CultureInfo culture]
		{
			get
			{
				if (culture == null)
				{
					throw new ArgumentNullException("culture");
				}
				return _innerList.FirstOrDefault((Language x) => object.Equals(x.Culture, culture));
			}
		}

		public Language Default
		{
			get
			{
				if (Lang.Default != null)
				{
					return this[Lang.Default];
				}
				return null;
			}
			set
			{
				Lang.Default = value.Culture;
			}
		}

		public Language Current
		{
			get
			{
				return this[Lang.Current];
			}
			set
			{
				Lang.Current = value.Culture;
			}
		}

		public int Count => _innerList.Count;

		bool ICollection<Language>.IsReadOnly => true;

		public bool Contains(Language language)
		{
			return _innerList.Contains(language);
		}

		internal void Add(Language language)
		{
			_innerList.Add(language);
		}

		internal bool Remove(Language language)
		{
			return _innerList.Remove(language);
		}

		internal void Clear()
		{
			_innerList.Clear();
		}

		void ICollection<Language>.Add(Language language)
		{
			_innerList.Add(language);
		}

		bool ICollection<Language>.Remove(Language language)
		{
			return _innerList.Remove(language);
		}

		void ICollection<Language>.Clear()
		{
			_innerList.Clear();
		}

		void ICollection<Language>.CopyTo(Language[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		IEnumerator<Language> IEnumerable<Language>.GetEnumerator()
		{
			return ((IEnumerable<Language>)_innerList).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Language>)_innerList).GetEnumerator();
		}
	}
}
