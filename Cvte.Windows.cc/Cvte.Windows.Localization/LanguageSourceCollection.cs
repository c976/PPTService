using System;
using System.Collections;
using System.Collections.Generic;

namespace Cvte.Windows.Localization
{
	public class LanguageSourceCollection : ICollection<ILanguageSource>, IEnumerable<ILanguageSource>, IEnumerable
	{
		private readonly Action<ILanguageSource> _addLanguage;

		private readonly List<ILanguageSource> _sources = new List<ILanguageSource>();

		public int Count => _sources.Count;

		bool ICollection<ILanguageSource>.IsReadOnly => false;

		internal LanguageSourceCollection(Action<ILanguageSource> addLanguage)
		{
			_addLanguage = addLanguage;
		}

		public void Add(ILanguageSource language)
		{
			while (true)
			{
				if (4 != 0)
				{
					if (language == null)
					{
						goto IL_0007;
					}
					goto IL_0012;
				}
				return;
				IL_0007:
				if (3 != 0)
				{
					throw new ArgumentNullException("language");
				}
				goto IL_0012;
				IL_0012:
				if (7 != 0)
				{
					_sources.Add(language);
					if (6 != 0)
					{
						break;
					}
					continue;
				}
				goto IL_0007;
			}
			_addLanguage(language);
		}

		public bool Contains(ILanguageSource language)
		{
			return _sources.Contains(language);
		}

		void ICollection<ILanguageSource>.CopyTo(ILanguageSource[] array, int arrayIndex)
		{
			_sources.CopyTo(array, arrayIndex);
		}

		bool ICollection<ILanguageSource>.Remove(ILanguageSource language)
		{
			throw new NotSupportedException();
		}

		void ICollection<ILanguageSource>.Clear()
		{
			throw new NotSupportedException();
		}

		public IEnumerator<ILanguageSource> GetEnumerator()
		{
			return _sources.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _sources.GetEnumerator();
		}
	}
}
