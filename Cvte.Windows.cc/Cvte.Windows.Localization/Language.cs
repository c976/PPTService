using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cvte.Windows.Localization
{
	[DebuggerDisplay("Culture={Culture.Name}, Count={_langDictionary.Count}")]
	public sealed class Language : IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		private readonly List<ILanguageReader> _languageReaderList = new List<ILanguageReader>();

		private readonly Dictionary<string, string> _langDictionary = new Dictionary<string, string>(256);

		private readonly ManualResetEvent _resetEvent = new ManualResetEvent(true);

		public CultureInfo Culture { get; private set; }

		public bool IsReady { get; private set; }

		public string this[string key]
		{
			get
			{
				return Get(key);
			}
			set
			{
				_langDictionary[key] = value;
			}
		}

		public Language(CultureInfo culture, ILanguageReader languageReader)
			: this(culture, languageReader, false)
		{
		}

		public Language(CultureInfo culture, ILanguageReader languageReader, bool read)
		{
			Culture = culture;
			_languageReaderList.Add(languageReader);
			if (read)
			{
				ReadFiles(null, null);
			}
		}

		public void Load()
		{
			ReadFiles(null, null);
		}

		public void Load(Action action)
		{
			ReadFiles(null, action);
		}

		public void Merge(Language language)
		{
			Merge(language, false);
		}

		public void Merge(Language language, bool read)
		{
			Merge(language, read, null);
		}

		public void Merge(Language language, Action action)
		{
			Merge(language, true, action);
		}

		private void Merge(Language language, bool read, Action action)
		{
			while (true)
			{
				if (language != null)
				{
					if (object.Equals(language.Culture, Culture))
					{
						if (2 == 0)
						{
							continue;
						}
						break;
					}
					if (-1 != 0)
					{
						throw new ArgumentException("合并的 Language 对象必须具有同种区域属性。");
					}
				}
				throw new ArgumentNullException("language");
			}
			List<ILanguageReader>.Enumerator enumerator = language._languageReaderList.GetEnumerator();
			try
			{
				while (true)
				{
					if (false || 6 != 0)
					{
					}
					while (0 == 0)
					{
						if (enumerator.MoveNext())
						{
							ILanguageReader current = enumerator.Current;
							if (!_languageReaderList.Contains(current))
							{
								_languageReaderList.Add(current);
							}
							continue;
						}
						goto end_IL_004e;
					}
					continue;
					end_IL_004e:
					break;
				}
			}
			finally
			{
				if (7 != 0)
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
			}
			if (read)
			{
				ReadFiles(language._languageReaderList, action);
			}
			else
			{
				IsReady = false;
			}
		}

		private void ReadFiles(IEnumerable<ILanguageReader> readers, Action action)
		{
			IsReady = false;
			_resetEvent.Reset();
			IsReady = false;
			if (false)
			{
				return;
			}
			Task task = new Task(() =>
			{
				ReadInner(readers);
			});
			task.Start();
			if (action != null)
			{
				task.ContinueWith((Task task2) =>
				{
					action();
				});
			}
		}

		private void ReadInner(IEnumerable<ILanguageReader> specifiedReaders)
		{
			try
			{
				IEnumerable<ILanguageReader> obj = specifiedReaders ?? _languageReaderList;
				IEnumerable<ILanguageReader> enumerable;
				if (4u != 0)
				{
					enumerable = obj;
				}
				foreach (ILanguageReader item in enumerable)
				{
					try
					{
						Dictionary<string, string> dictionary = item.Read(Culture);
						foreach (KeyValuePair<string, string> item2 in dictionary)
						{
							_langDictionary[item2.Key] = item2.Value;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			finally
			{
				IsReady = true;
				_resetEvent.Set();
				IsReady = true;
			}
		}

		IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
		{
			lock (_langDictionary)
			{
				return _langDictionary.Skip(0).GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, string>>)this).GetEnumerator();
		}

		[DebuggerStepThrough]
		public string Get(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return GetLocalWithLocker(key, false);
		}

		[DebuggerStepThrough]
		public string Get(string key, params object[] args)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return string.Format(GetLocalWithLocker(key, false), args);
		}

		[DebuggerStepThrough]
		public bool TryGet(string key, out string value)
		{
			while (true)
			{
				if (key == null)
				{
					if (0 == 0)
					{
						throw new ArgumentNullException("key");
					}
				}
				else if (4 != 0)
				{
					break;
				}
			}
			value = GetLocalWithLocker(key, true);
			return value != null;
		}

		[DebuggerStepThrough]
		internal string GetLocalWithLocker(string key, bool isTrying)
		{
			while (2 != 0)
			{
				bool flag = _resetEvent.WaitOne();
				while (4 != 0)
				{
					flag = IsReady;
					if (false)
					{
						continue;
					}
					goto IL_0017;
				}
				break;
				IL_0017:
				if (!flag)
				{
					if (0 == 0)
					{
						Load();
						flag = _resetEvent.WaitOne();
						break;
					}
					continue;
				}
				break;
			}
			return GetLocal(key, isTrying);
		}

		[DebuggerStepThrough]
		private string GetLocal(string key, bool isTrying)
		{
			if (0 == 0)
			{
				string value;
				bool flag = _langDictionary.TryGetValue(key, out value);
				if (5 != 0)
				{
					if (flag)
					{
						return value;
					}
					if (false)
					{
						goto IL_002b;
					}
					flag = isTrying;
				}
				if (flag)
				{
					goto IL_002b;
				}
			}
			if (true)
			{
				return Lang.HandleKeyNotFound(Culture, key);
			}
			goto IL_002b;
			IL_002b:
			return null;
		}
	}
}
