using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Cvte.Windows.Localization
{
	[DebuggerDisplay("Default={Default.Culture}, Current={Current.Culture}, Count={Count}")]
	public class LanguageCollection : ICollection<Language>, IEnumerable<Language>, IEnumerable
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1
		{
			public string name;

			public bool _003Cget_Item_003Eb__0(Language x)
			{
				return x.Culture.Name == name;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4
		{
			public CultureInfo culture;

			public bool _003Cget_Item_003Eb__3(Language x)
			{
				return object.Equals(x.Culture, culture);
			}
		}

		private readonly List<Language> _innerList = new List<Language>();

		public Language this[string name]
		{
			get
			{
				_003C_003Ec__DisplayClass1 obj2 = default(_003C_003Ec__DisplayClass1);
				while (true)
				{
					_003C_003Ec__DisplayClass1 obj = new _003C_003Ec__DisplayClass1();
					if (0 == 0)
					{
						obj2 = obj;
					}
					while (0 == 0)
					{
						obj2.name = name;
						if (4 != 0)
						{
							if (obj2.name == null)
							{
								throw new ArgumentNullException("name");
							}
							return _innerList.FirstOrDefault(obj2._003Cget_Item_003Eb__0);
						}
					}
				}
			}
		}

		public Language this[CultureInfo culture]
		{
			get
			{
				_003C_003Ec__DisplayClass4 obj2 = default(_003C_003Ec__DisplayClass4);
				while (true)
				{
					_003C_003Ec__DisplayClass4 obj = new _003C_003Ec__DisplayClass4();
					if (0 == 0)
					{
						obj2 = obj;
					}
					while (0 == 0)
					{
						obj2.culture = culture;
						if (4 != 0)
						{
							if (obj2.culture == null)
							{
								throw new ArgumentNullException("culture");
							}
							return _innerList.FirstOrDefault(obj2._003Cget_Item_003Eb__3);
						}
					}
				}
			}
		}

		public Language Default
		{
			get
			{
				if (Lang.Default == null)
				{
					return null;
				}
				return this[Lang.Default];
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
