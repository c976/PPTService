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
		private static readonly DependencyPropertyKey KeyPropertyKey;

		private static readonly DependencyProperty KeyProperty;

		private static readonly DependencyPropertyKey ValuePropertyKey;

		public static readonly DependencyProperty ValueProperty;

		private static readonly List<WeakReference> UsingLangs;

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
			do
			{
				if (1 == 0)
				{
					continue;
				}
				Task.Factory.StartNew(() =>
				{
					List<ReferencedLang> list = new List<ReferencedLang>();
					WeakReference[] array = UsingLangs.ToArray();
					int num = 0;
					while (true)
					{
						int num2 = num;
						int num3 = array.Length;
						do
						{
							if (num2 >= num3)
							{
								foreach (IGrouping<Dispatcher, ReferencedLang> item in list.ToLookup((ReferencedLang lang) => lang.Dispatcher))
								{
									IGrouping<Dispatcher, ReferencedLang> group = item;
									group.Key.BeginInvoke(DispatcherPriority.Loaded, (Action)(() =>
									{
										if (-1 != 0)
										{
											IEnumerator<ReferencedLang> enumerator2 = group.GetEnumerator();
											IEnumerator<ReferencedLang> enumerator3;
											if (4u != 0)
											{
												enumerator3 = enumerator2;
											}
											try
											{
												while (true)
												{
													if (!enumerator3.MoveNext())
													{
														if (0 == 0 && 0 == 0)
														{
															break;
														}
													}
													else
													{
														ReferencedLang current2 = enumerator3.Current;
														if (0 == 0)
														{
															current2.UpdateValue();
														}
													}
												}
											}
											finally
											{
												enumerator3?.Dispose();
											}
										}
									}));
								}
								return;
							}
							WeakReference weakReference = array[num];
							ReferencedLang referencedLang = (ReferencedLang)weakReference.Target;
							if (referencedLang == null)
							{
								UsingLangs.Remove(weakReference);
							}
							else
							{
								list.Add(referencedLang);
							}
							while (false)
							{
							}
							num2 = num;
							num3 = 1;
						}
						while (num3 == 0);
						num = num2 + num3;
					}
				});
			}
			while (false ? true : false);
		}

		static ReferencedLang()
		{
			while (true)
			{
				if (0 == 0)
				{
					KeyPropertyKey = DependencyProperty.RegisterReadOnly("Key", typeof(string), typeof(ReferencedLang), new PropertyMetadata((object)null));
					if (false)
					{
						continue;
					}
					if (false)
					{
						break;
					}
					if (0 == 0)
					{
						KeyProperty = KeyPropertyKey.DependencyProperty;
						if (0 == 0)
						{
							ValuePropertyKey = DependencyProperty.RegisterReadOnly("Value", typeof(string), typeof(ReferencedLang), new PropertyMetadata((object)null));
						}
					}
				}
				ValueProperty = ValuePropertyKey.DependencyProperty;
				UsingLangs = new List<WeakReference>(256);
				break;
			}
		}
	}
}
