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

		public static event EventHandler<CurrentLanguageChangedEventArgs> CurrentLanguageChanged;

		public static event LanguageKeyNotFoundEventHandler CurrentLanguageKeyNotFound;

		public static event LanguageKeyNotFoundEventHandler DefaultLanguageKeyNotFound;

		static Lang()
		{
			Languages = new LanguageCollection();
			Sources = new LanguageSourceCollection(AddLanguages);
			_current = Thread.CurrentThread.CurrentUICulture;
		}

		private static void AddLanguages(ILanguageSource source)
		{
			if (source == null)
			{
				return;
			}
			foreach (Language language2 in source.GetLanguages())
			{
				Language language = Languages[language2.Culture];
				if (language == null)
				{
					Languages.Add(language2);
				}
				else if (object.Equals(language2.Culture, Current))
				{
					Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
					Action action = (ReferencedLang.IsAnyLangItemUsing ? ((Action)(() =>
					{
						dispatcher.BeginInvoke(new Action(Update));
					})) : null);
					language.Merge(language2, action);
				}
				else
				{
					language.Merge(language2);
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
			value = GetWithDefaultValue(key, true);
			return value != null;
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
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			Binding binding = new Binding(propertyName);
			binding.Source = source;
			binding.Mode = BindingMode.OneWayToSource;
			Binding binding2 = binding;
			ReferencedLang referencedLang = new ReferencedLang(key);
			BindingOperations.SetBinding(referencedLang, ReferencedLang.ValueProperty, binding2);
			return referencedLang.UpdateValue();
		}

		[DebuggerStepThrough]
		private static string GetWithDefaultValue(string key, bool isTrying)
		{
			Language language = Languages.Current;
			Language language2 = Languages.Default;
			string text;
			if (language == null)
			{
				text = HandleKeyNotFound(Current, key);
				if (text != null)
				{
					return text;
				}
				language = language2;
			}
			text = language.GetLocalWithLocker(key, true);
			if (text != null)
			{
				return text;
			}
			if (object.ReferenceEquals(language2, language))
			{
				if (!isTrying)
				{
					return HandleKeyNotFound(Default, key);
				}
				return null;
			}
			if (language2 == null)
			{
				text = HandleKeyNotFound(Current, key);
				if (text != null)
				{
					return text;
				}
				if (!isTrying)
				{
					return HandleKeyNotFound(Default, key);
				}
				return null;
			}
			text = HandleKeyNotFound(Current, key);
			if (text != null)
			{
				return text;
			}
			text = language2.GetLocalWithLocker(key, true);
			if (text != null)
			{
				return text;
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
			Language language = Languages[culture];
			bool flag;
			LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler = ((flag = object.ReferenceEquals(Languages.Default, language)) ? DefaultLanguageKeyNotFound : CurrentLanguageKeyNotFound);
			if (languageKeyNotFoundEventHandler == null)
			{
				if (!flag)
				{
					return null;
				}
				throw new KeyNotFoundException($"区域 {culture} 中找不到语言 {key}。");
			}
			return languageKeyNotFoundEventHandler(language, new LanguageKeyNotFoundEventArgs(key, culture));
		}

		private static void RaiseCurrentLanguageChanged(CultureInfo oldCulture, CultureInfo newCulture)
		{
			CurrentLanguageChanged?.Invoke(null, new CurrentLanguageChangedEventArgs(oldCulture, newCulture));
		}
	}
}
