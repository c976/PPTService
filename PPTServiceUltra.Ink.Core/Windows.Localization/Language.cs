using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PPTServiceUltra.Ink.Core.ComponentModel;
using PPTServiceUltra.Ink.Core.Diagnostics.Trace;

namespace PPTServiceUltra.Ink.Core.Windows.Localization
{
	[DebuggerDisplay("Culture={Culture.Name}, Count={_langDictionary.Count}")]
	public sealed class Language : IEnumerable<KeyValuePair<string, string>>, IEnumerable
	{
		private int _readTaskCount;

		private readonly List<ILanguageReader> _languageReaderList = new List<ILanguageReader>();

		private readonly Dictionary<string, string> _langDictionary = new Dictionary<string, string>(256);

		private readonly ManualResetEvent _resetEvent = new ManualResetEvent(true);

		public CultureInfo Culture { get; private set; }

		public bool IsReady { get; private set; }

		private object LangDictionaryLocker => _langDictionary;

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
			if (language == null)
			{
				throw new ArgumentNullException("language");
			}
			if (!object.Equals(language.Culture?.Name, Culture?.Name))
			{
				throw new ArgumentException("合并的 Language 对象必须具有同种区域属性。");
			}
			foreach (ILanguageReader languageReader in language._languageReaderList)
			{
				if (_languageReaderList.Contains(languageReader))
				{
					if (DebuggingProperties.IsDebug)
					{
						throw new ArgumentException("指定的 ILanguageReader 已经存在。仅调试下抛出，请找德熙");
					}
				}
				else
				{
					_languageReaderList.Add(languageReader);
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
			Interlocked.Increment(ref _readTaskCount);
			try
			{
				IEnumerable<ILanguageReader> enumerable;
				lock (_languageReaderList)
				{
					enumerable = specifiedReaders ?? _languageReaderList;
					enumerable = enumerable.ToArray();
				}
				foreach (ILanguageReader item in enumerable)
				{
					try
					{
						Dictionary<string, string> dictionary = item.Read(Culture);
						lock (LangDictionaryLocker)
						{
							foreach (KeyValuePair<string, string> item2 in dictionary)
							{
								_langDictionary[item2.Key] = item2.Value;
							}
						}
					}
					catch (Exception arg)
					{
						Tracer.Error($"区域 {Culture} 的语言源读取失败，其原因是：{arg}");
					}
				}
			}
			finally
			{
				IsReady = true;
				Interlocked.Decrement(ref _readTaskCount);
				if (_readTaskCount == 0)
				{
					_resetEvent.Set();
				}
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

		public string Get(string key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return GetLocalWithLocker(key, false);
		}

		public string Get(string key, params object[] args)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			return string.Format(GetLocalWithLocker(key, false), args);
		}

		public bool TryGet(string key, out string value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			value = GetLocalWithLocker(key, true);
			return value != null;
		}

		internal string GetLocalWithLocker(string key, bool isTrying)
		{
			_resetEvent.WaitOne();
			if (!IsReady)
			{
				Load();
				_resetEvent.WaitOne();
			}
			return GetLocal(key, isTrying);
		}

		private string GetLocal(string key, bool isTrying)
		{
			string value;
			if (_langDictionary.TryGetValue(key, out value))
			{
				return value;
			}
			if (!isTrying)
			{
				return Lang.HandleKeyNotFound(Culture, key);
			}
			return null;
		}
	}
}
