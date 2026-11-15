using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.Localization
{
	public static class Lang
	{
		private static bool _isCurrentLangFirstLoaded;

		private static CultureInfo _current;

		public static LanguageSourceCollection Sources { get; private set; }

		public static LanguageCollection Languages { get; }

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
				if (object.Equals(_current?.Name, value.Name))
				{
					if (_isCurrentLangFirstLoaded)
					{
						return;
					}
					_isCurrentLangFirstLoaded = true;
				}
				Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
				Action action = (ReferencedLang.IsAnyLangItemUsing ? ((Action)(() =>
				{
					dispatcher.BeginInvoke(new Action(Update));
				})) : null);
				CultureInfo current = _current;
				_current = value;
				Languages.Current?.Load(action);
				RaiseCurrentLanguageChanged(current, value);
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
				else if (object.Equals(language2.Culture.Name, Current?.Name))
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

		[Pure]
		public static string Get(string key)
		{
			return GetWithDefaultValue(key, false);
		}

		[Pure]
		public static string Get(string key, params object[] args)
		{
			string withDefaultValue = GetWithDefaultValue(key, false);
			if (string.IsNullOrEmpty(withDefaultValue))
			{
				return withDefaultValue;
			}
			return string.Format(withDefaultValue, args);
		}

		[Pure]
		public static bool TryGet(string key, out string value)
		{
			value = GetWithDefaultValue(key, true);
			return value != null;
		}

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
			Binding binding = new Binding(ReferencedLang.ValueProperty.Name)
			{
				Source = referencedLang,
				Mode = BindingMode.OneWay
			};
			BindingOperations.SetBinding(source, property, binding);
			return result;
		}

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
			Binding binding = new Binding(propertyName)
			{
				Source = source,
				Mode = BindingMode.OneWayToSource
			};
			ReferencedLang referencedLang = new ReferencedLang(key);
			BindingOperations.SetBinding(referencedLang, ReferencedLang.ValueProperty, binding);
			return referencedLang.UpdateValue();
		}

		private static string GetWithDefaultValue(string key, bool isTrying)
		{
			Language language = Languages.Current;
			Language language2 = Languages.Default;
			string text;
			if (language == null)
			{
				text = HandleKeyNotFound(Current, key, isTrying);
				if (text != null)
				{
					return text;
				}
				language = language2;
			}
			text = language?.GetLocalWithLocker(key, true);
			if (text != null)
			{
				return text;
			}
			if (language2 == language)
			{
				return HandleKeyNotFound(Default, key, isTrying);
			}
			if (language2 == null)
			{
				text = HandleKeyNotFound(Current, key, isTrying);
				if (text != null)
				{
					return text;
				}
				return HandleKeyNotFound(Default, key, isTrying);
			}
			text = HandleKeyNotFound(Current, key, isTrying);
			if (text != null)
			{
				return text;
			}
			text = language2.GetLocalWithLocker(key, true);
			if (text != null)
			{
				return text;
			}
			return HandleKeyNotFound(Default, key, isTrying);
		}

		[DebuggerStepThrough]
		internal static string HandleKeyNotFound(CultureInfo culture, string key, bool isTrying = false)
		{
			Language language = Languages[culture];
			bool flag = Languages.Default == language;
			LanguageKeyNotFoundEventHandler languageKeyNotFoundEventHandler = (flag ? DefaultLanguageKeyNotFound : CurrentLanguageKeyNotFound);
			if (languageKeyNotFoundEventHandler == null)
			{
				if (!flag | isTrying)
				{
					return null;
				}
				throw new KeyNotFoundException($"区域 {culture} 中找不到语言 {key}。");
			}
			string result = languageKeyNotFoundEventHandler(language, new LanguageKeyNotFoundEventArgs(key, culture));
			if (!isTrying)
			{
				return result;
			}
			return null;
		}

		private static void RaiseCurrentLanguageChanged(CultureInfo oldCulture, CultureInfo newCulture)
		{
			CurrentLanguageChanged?.Invoke(null, new CurrentLanguageChangedEventArgs(oldCulture, newCulture));
		}
	}
}
