using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Cvte.Windows.Localization
{
	public static class Lang
	{
		private static CultureInfo _current;

		private static EventHandler<CurrentLanguageChangedEventArgs> m_CurrentLanguageChanged;

		private static LanguageKeyNotFoundEventHandler m_CurrentLanguageKeyNotFound;

		private static LanguageKeyNotFoundEventHandler m_DefaultLanguageKeyNotFound;

		public static LanguageSourceCollection Sources { get; private set; }

		public static LanguageCollection Languages { get; private set; }

		public static CultureInfo Default { get; set; }

		public static CultureInfo Current
		{
			get
			{
				return _current;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "当前语言区域不能被设置为 null。");
				}
				if (!object.Equals(_current, value))
				{
					Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
					Action action = (ReferencedLang.IsAnyLangItemUsing ? ((Action)(() =>
					{
						dispatcher.BeginInvoke(new Action(Update));
					})) : null);
					CultureInfo current = _current;
					_current = value;
					if (Languages.Current != null)
					{
						Languages.Current.Load(action);
					}
					RaiseCurrentLanguageChanged(current, value);
				}
			}
		}

		public static event EventHandler<CurrentLanguageChangedEventArgs> CurrentLanguageChanged
		{
			add
			{
				while (true)
				{
					EventHandler<CurrentLanguageChangedEventArgs> eventHandler = Lang.m_CurrentLanguageChanged;
					while (0 == 0)
					{
						EventHandler<CurrentLanguageChangedEventArgs> eventHandler2 = eventHandler;
						while (true)
						{
							EventHandler<CurrentLanguageChangedEventArgs> value2;
							if (0 == 0)
							{
								value2 = (EventHandler<CurrentLanguageChangedEventArgs>)Delegate.Combine(eventHandler2, value);
							}
							if (0 == 0)
							{
								eventHandler = Interlocked.CompareExchange(ref Lang.m_CurrentLanguageChanged, value2, eventHandler2);
								if ((object)eventHandler != eventHandler2)
								{
									break;
								}
								if (0 == 0)
								{
									return;
								}
							}
						}
					}
				}
			}
			remove
			{
				while (true)
				{
					EventHandler<CurrentLanguageChangedEventArgs> eventHandler = Lang.m_CurrentLanguageChanged;
					while (0 == 0)
					{
						EventHandler<CurrentLanguageChangedEventArgs> eventHandler2 = eventHandler;
						while (true)
						{
							EventHandler<CurrentLanguageChangedEventArgs> value2;
							if (0 == 0)
							{
								value2 = (EventHandler<CurrentLanguageChangedEventArgs>)Delegate.Remove(eventHandler2, value);
							}
							if (0 == 0)
							{
								eventHandler = Interlocked.CompareExchange(ref Lang.m_CurrentLanguageChanged, value2, eventHandler2);
								if ((object)eventHandler != eventHandler2)
								{
									break;
								}
								if (0 == 0)
								{
									return;
								}
							}
						}
					}
				}
			}
		}

		public static event LanguageKeyNotFoundEventHandler CurrentLanguageKeyNotFound
		{
			add
			{
				while (true)
				{
					LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler = Lang.m_CurrentLanguageKeyNotFound;
					while (0 == 0)
					{
						LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler2 = languageKeyNotFoundEventHandler;
						while (true)
						{
							LanguageKeyNotFoundEventHandler value2;
							if (0 == 0)
							{
								value2 = (LanguageKeyNotFoundEventHandler)Delegate.Combine(languageKeyNotFoundEventHandler2, value);
							}
							if (0 == 0)
							{
								languageKeyNotFoundEventHandler = Interlocked.CompareExchange(ref Lang.m_CurrentLanguageKeyNotFound, value2, languageKeyNotFoundEventHandler2);
								if ((object)languageKeyNotFoundEventHandler != languageKeyNotFoundEventHandler2)
								{
									break;
								}
								if (0 == 0)
								{
									return;
								}
							}
						}
					}
				}
			}
			remove
			{
				while (true)
				{
					LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler = Lang.m_CurrentLanguageKeyNotFound;
					while (0 == 0)
					{
						LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler2 = languageKeyNotFoundEventHandler;
						while (true)
						{
							LanguageKeyNotFoundEventHandler value2;
							if (0 == 0)
							{
								value2 = (LanguageKeyNotFoundEventHandler)Delegate.Remove(languageKeyNotFoundEventHandler2, value);
							}
							if (0 == 0)
							{
								languageKeyNotFoundEventHandler = Interlocked.CompareExchange(ref Lang.m_CurrentLanguageKeyNotFound, value2, languageKeyNotFoundEventHandler2);
								if ((object)languageKeyNotFoundEventHandler != languageKeyNotFoundEventHandler2)
								{
									break;
								}
								if (0 == 0)
								{
									return;
								}
							}
						}
					}
				}
			}
		}

		public static event LanguageKeyNotFoundEventHandler DefaultLanguageKeyNotFound
		{
			add
			{
				while (true)
				{
					LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler = Lang.m_DefaultLanguageKeyNotFound;
					while (0 == 0)
					{
						LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler2 = languageKeyNotFoundEventHandler;
						while (true)
						{
							LanguageKeyNotFoundEventHandler value2;
							if (0 == 0)
							{
								value2 = (LanguageKeyNotFoundEventHandler)Delegate.Combine(languageKeyNotFoundEventHandler2, value);
							}
							if (0 == 0)
							{
								languageKeyNotFoundEventHandler = Interlocked.CompareExchange(ref Lang.m_DefaultLanguageKeyNotFound, value2, languageKeyNotFoundEventHandler2);
								if ((object)languageKeyNotFoundEventHandler != languageKeyNotFoundEventHandler2)
								{
									break;
								}
								if (0 == 0)
								{
									return;
								}
							}
						}
					}
				}
			}
			remove
			{
				while (true)
				{
					LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler = Lang.m_DefaultLanguageKeyNotFound;
					while (0 == 0)
					{
						LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler2 = languageKeyNotFoundEventHandler;
						while (true)
						{
							LanguageKeyNotFoundEventHandler value2;
							if (0 == 0)
							{
								value2 = (LanguageKeyNotFoundEventHandler)Delegate.Remove(languageKeyNotFoundEventHandler2, value);
							}
							if (0 == 0)
							{
								languageKeyNotFoundEventHandler = Interlocked.CompareExchange(ref Lang.m_DefaultLanguageKeyNotFound, value2, languageKeyNotFoundEventHandler2);
								if ((object)languageKeyNotFoundEventHandler != languageKeyNotFoundEventHandler2)
								{
									break;
								}
								if (0 == 0)
								{
									return;
								}
							}
						}
					}
				}
			}
		}

		static Lang()
		{
			if (true)
			{
				if (0 == 0)
				{
					Languages = new LanguageCollection();
				}
				goto IL_000a;
			}
			goto IL_0046;
			IL_000a:
			Sources = new LanguageSourceCollection(AddLanguages);
			goto IL_0046;
			IL_0046:
			_current = Thread.CurrentThread.CurrentUICulture;
			if (0 == 0 && 0 == 0)
			{
				return;
			}
			goto IL_000a;
		}

		private static void AddLanguages(ILanguageSource source)
		{
			if (source == null)
			{
				return;
			}
			using (IEnumerator<Language> enumerator = source.GetLanguages().GetEnumerator())
			{
				Language current = default(Language);
				while (true)
				{
					if (3 != 0)
					{
						Language language;
						if (!enumerator.MoveNext())
						{
							if (0 == 0)
							{
								break;
							}
						}
						else
						{
							current = enumerator.Current;
							language = Languages[current.Culture];
							if (language == null)
							{
								goto IL_0044;
							}
							if (5 == 0 || !object.Equals(current.Culture, Current))
							{
								language.Merge(current);
								continue;
							}
						}
						Dispatcher dispatcher = default(Dispatcher);
						do
						{
							object obj;
							if (2 != 0)
							{
								dispatcher = Dispatcher.CurrentDispatcher;
								if (6 == 0)
								{
									continue;
								}
								if (!ReferencedLang.IsAnyLangItemUsing)
								{
									obj = null;
									goto IL_0093;
								}
							}
							obj = (Action)(() =>
							{
								dispatcher.BeginInvoke(new Action(Update));
							});
							goto IL_0093;
							IL_0093:
							Action action = (Action)obj;
							language.Merge(current, action);
						}
						while (false);
						continue;
					}
					goto IL_0044;
					IL_0044:
					Languages.Add(current);
				}
			}
		}

		internal static void Update()
		{
		}

		[DebuggerStepThrough]
		public static string Get(string key)
		{
			return GetWithDefaultValue(key, false);
		}

		[DebuggerStepThrough]
		public static string Get(string key, params object[] args)
		{
			return string.Format(GetWithDefaultValue(key, false), args);
		}

		[DebuggerStepThrough]
		public static bool TryGet(string key, out string value)
		{
			if (0 == 0)
			{
				value = GetWithDefaultValue(key, true);
			}
			bool flag = value == null;
			do
			{
				flag = !flag;
			}
			while (false);
			return flag;
		}

		[DebuggerStepThrough]
		public static string Reference(string key, DependencyObject source, DependencyProperty property)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			ReferencedLang referencedLang = new ReferencedLang(key);
			string result = referencedLang.UpdateValue();
			Binding binding = new Binding(ReferencedLang.ValueProperty.Name);
			binding.Source = referencedLang;
			binding.Mode = BindingMode.OneWay;
			Binding binding2 = binding;
			BindingOperations.SetBinding(source, property, binding2);
			return result;
		}

		[DebuggerStepThrough]
		[Obsolete("在 .NET Framework 4.5 及之前的版本中，使用此方法会导致指定的属性被额外赋值 null 一次，即“原值→null→目标值”。建议使用 string Reference(string, DependencyObject, DependencyProperty) 代替；如无需语言更改，请使用 string Lang.Get(string)。")]
		public static string Reference(string key, object source, string propertyName)
		{
			ReferencedLang referencedLang;
			while (0 == 0)
			{
				if (key == null)
				{
					throw new ArgumentNullException("key");
				}
				if (source == null)
				{
					throw new ArgumentNullException("source");
				}
				Binding binding;
				while (7 != 0)
				{
					if (0 == 0 && propertyName == null)
					{
						throw new ArgumentNullException("propertyName");
					}
					binding = new Binding(propertyName);
					binding.Source = source;
					binding.Mode = BindingMode.OneWayToSource;
					if (false)
					{
						continue;
					}
					goto IL_0047;
				}
				continue;
				IL_0047:
				Binding binding2 = binding;
				referencedLang = new ReferencedLang(key);
				BindingOperations.SetBinding(referencedLang, ReferencedLang.ValueProperty, binding2);
				break;
			}
			return referencedLang.UpdateValue();
		}

		[DebuggerStepThrough]
		private static string GetWithDefaultValue(string key, bool isTrying)
		{
			string localWithLocker;
			while (true)
			{
				Language language = Languages.Current;
				Language language3;
				do
				{
					Language language2 = Languages.Default;
					if (2u != 0)
					{
						language3 = language2;
					}
				}
				while (false);
				if (language != null)
				{
					goto IL_0044;
				}
				goto IL_010a;
				IL_0044:
				localWithLocker = language.GetLocalWithLocker(key, true);
				if (localWithLocker != null)
				{
					return localWithLocker;
				}
				if (-1 == 0)
				{
					break;
				}
				if (object.ReferenceEquals(language3, language))
				{
					if (!isTrying)
					{
						return HandleKeyNotFound(Default, key);
					}
					return null;
				}
				if (false)
				{
					continue;
				}
				if (language3 == null)
				{
					localWithLocker = HandleKeyNotFound(Current, key);
					if (localWithLocker != null)
					{
						if (6 == 0)
						{
							goto IL_010a;
						}
						return localWithLocker;
					}
					if (!isTrying)
					{
						return HandleKeyNotFound(Default, key);
					}
					if (0 == 0)
					{
						return null;
					}
				}
				else
				{
					localWithLocker = HandleKeyNotFound(Current, key);
				}
				if (localWithLocker != null)
				{
					return localWithLocker;
				}
				localWithLocker = language3.GetLocalWithLocker(key, true);
				break;
				IL_010a:
				do
				{
					localWithLocker = HandleKeyNotFound(Current, key);
					if (localWithLocker != null)
					{
						return localWithLocker;
					}
				}
				while (false);
				language = language3;
				goto IL_0044;
			}
			if (localWithLocker != null)
			{
				return localWithLocker;
			}
			if (!isTrying)
			{
				return HandleKeyNotFound(Default, key);
			}
			return null;
		}

		[DebuggerStepThrough]
		internal static string HandleKeyNotFound(CultureInfo culture, string key)
		{
			while (true)
			{
				Language language = Languages[culture];
				Language language2;
				if (true)
				{
					language2 = language;
				}
				while (true)
				{
					bool flag;
					bool flag2;
					if (0 == 0)
					{
						flag = object.ReferenceEquals(Languages.Default, language2);
						if (0 == 0)
						{
							flag2 = flag;
							flag = flag2;
						}
						goto IL_0033;
					}
					goto IL_0048;
					IL_0048:
					LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler;
					if (languageKeyNotFoundEventHandler == null)
					{
						flag = flag2;
						if (2 != 0)
						{
							if (!flag)
							{
								return null;
							}
							throw new KeyNotFoundException($"区域 {culture} 中找不到语言 {key}。");
						}
						goto IL_0033;
					}
					return languageKeyNotFoundEventHandler(language2, new LanguageKeyNotFoundEventArgs(key, culture));
					IL_0033:
					LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler2;
					if (!flag)
					{
						if (false)
						{
							break;
						}
						if (8 == 0)
						{
							continue;
						}
						languageKeyNotFoundEventHandler2 = Lang.m_CurrentLanguageKeyNotFound;
					}
					else
					{
						languageKeyNotFoundEventHandler2 = Lang.m_DefaultLanguageKeyNotFound;
					}
					languageKeyNotFoundEventHandler = languageKeyNotFoundEventHandler2;
					goto IL_0048;
				}
			}
		}

		private static void RaiseCurrentLanguageChanged(CultureInfo oldCulture, CultureInfo newCulture)
		{
			EventHandler<CurrentLanguageChangedEventArgs> currentLanguageChanged = default(EventHandler<CurrentLanguageChangedEventArgs>);
			if (true)
			{
				if (0 == 0)
				{
					currentLanguageChanged = Lang.m_CurrentLanguageChanged;
				}
				goto IL_0026;
			}
			goto IL_0029;
			IL_0029:
			currentLanguageChanged(null, new CurrentLanguageChangedEventArgs(oldCulture, newCulture));
			goto IL_001c;
			IL_001c:
			if (false ? true : false)
			{
				goto IL_0026;
			}
			return;
			IL_0026:
			if (currentLanguageChanged == null)
			{
				goto IL_001c;
			}
			goto IL_0029;
		}
	}
}
