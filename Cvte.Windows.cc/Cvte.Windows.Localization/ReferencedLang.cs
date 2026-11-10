using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Cvte.Windows.Localization
{
	[DebuggerDisplay("{Key}, {Value}")]
	[TypeConverter(typeof(ReferencedLangConverter))]
	public sealed class ReferencedLang : DependencyObject
	{
		private static readonly DependencyPropertyKey KeyPropertyKey = DependencyProperty.RegisterReadOnly("Key", typeof(string), typeof(ReferencedLang), new PropertyMetadata((object)null));

		private static readonly DependencyProperty KeyProperty = KeyPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ValuePropertyKey = DependencyProperty.RegisterReadOnly("Value", typeof(string), typeof(ReferencedLang), new PropertyMetadata((object)null));

		public static readonly DependencyProperty ValueProperty = ValuePropertyKey.DependencyProperty;

		private static readonly List<WeakReference> UsingLangs = new List<WeakReference>(256);

		public string Key
		{
			get
			{
				return (string)GetValue(KeyProperty);
			}
			private set
			{
				SetValue(KeyPropertyKey, value);
			}
		}

		public string Value
		{
			get
			{
				return (string)GetValue(ValueProperty);
			}
			private set
			{
				SetValue(ValuePropertyKey, value);
			}
		}

		internal static bool IsAnyLangItemUsing => UsingLangs.Any();

		public ReferencedLang(string key)
		{
			Key = key;
			UsingLangs.Add(new WeakReference(this));
		}

		internal string UpdateValue()
		{
			return Value = Lang.Get(Key);
		}

		internal static void UpdateValues()
		{
			Task.Factory.StartNew(() =>
			{
				List<ReferencedLang> list = new List<ReferencedLang>();
				WeakReference[] array = UsingLangs.ToArray();
				foreach (WeakReference weakReference in array)
				{
					ReferencedLang referencedLang = (ReferencedLang)weakReference.Target;
					if (referencedLang == null)
					{
						UsingLangs.Remove(weakReference);
					}
					else
					{
						list.Add(referencedLang);
					}
				}
				foreach (IGrouping<Dispatcher, ReferencedLang> item in list.ToLookup((ReferencedLang lang) => lang.Dispatcher))
				{
					IGrouping<Dispatcher, ReferencedLang> group = item;
					group.Key.BeginInvoke(DispatcherPriority.Loaded, (Action)(() =>
					{
						foreach (ReferencedLang item2 in group)
						{
							item2.UpdateValue();
						}
					}));
				}
			});
		}
	}
}
