using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Cvte.Windows.Localization
{
	public class DesignTimeXamlLang : DependencyObject
	{
		public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(Uri), typeof(DesignTimeXamlLang), new PropertyMetadata(null, PropertyChangedCallback));

		public static readonly DependencyProperty CultureProperty = DependencyProperty.RegisterAttached("Culture", typeof(CultureInfo), typeof(DesignTimeXamlLang), new PropertyMetadata(null, PropertyChangedCallback));

		private static Window _designTimeWindow;

		public Uri Source
		{
			get
			{
				return (Uri)GetValue(SourceProperty);
			}
			set
			{
				SetValue(SourceProperty, value);
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return (CultureInfo)GetValue(CultureProperty);
			}
			set
			{
				SetValue(CultureProperty, value);
			}
		}

		public bool Debug
		{
			get
			{
				return DebugInner;
			}
			set
			{
				DebugInner = value;
			}
		}

		private static bool DebugInner { get; set; }

		public static void SetSource(DependencyObject element, Uri value)
		{
			element.SetValue(SourceProperty, value);
		}

		public static Uri GetSource(DependencyObject element)
		{
			return (Uri)element.GetValue(SourceProperty);
		}

		public static void SetCulture(DependencyObject element, CultureInfo value)
		{
			element.SetValue(CultureProperty, value);
		}

		public static CultureInfo GetCulture(DependencyObject element)
		{
			return (CultureInfo)element.GetValue(CultureProperty);
		}

		private static void PropertyChangedCallback(DependencyObject dp, DependencyPropertyChangedEventArgs args)
		{
			if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				UpdateDesignTimeLanguage(dp);
			}
		}

		private static void DebugInDesignTime(bool forceShow, string text, params object[] args)
		{
			if (!DebugInner)
			{
				if (5 == 0)
				{
				}
				return;
			}
			bool num = Debugger.IsAttached;
			if (0 == 0)
			{
				if (true)
				{
					if (0 == 0)
					{
						num = num && forceShow;
					}
					forceShow = num;
					if (2 == 0)
					{
						goto IL_0060;
					}
					num = forceShow;
				}
				if (!num && !DesignerProperties.GetIsInDesignMode(new DependencyObject()))
				{
					return;
				}
				if (_designTimeWindow == null)
				{
					goto IL_0154;
				}
				num = _designTimeWindow.IsLoaded;
			}
			if (num)
			{
				goto IL_00dc;
			}
			goto IL_0154;
			IL_0060:
			Window window = default(Window);
			window.Title = "EasiNote 多语言设计时调试窗口";
			window.Width = 320.0;
			do
			{
				window.Height = 540.0;
			}
			while (5 == 0);
			window.Left = 100.0;
			window.Top = 100.0;
			window.Content = new TextBox
			{
				VerticalContentAlignment = VerticalAlignment.Top,
				TextAlignment = TextAlignment.Left,
				IsReadOnly = true
			};
			_designTimeWindow = window;
			_designTimeWindow.Show();
			goto IL_00dc;
			IL_00dc:
			((TextBox)_designTimeWindow.Content).Text += string.Format(text + "\r\n", args);
			return;
			IL_0154:
			window = new Window();
			goto IL_0060;
		}

		public DesignTimeXamlLang()
		{
			DebugInDesignTime(true, "设计时语言模块初始化……");
		}

		private static void UpdateDesignTimeLanguage(DependencyObject dp)
		{
			Uri source = GetSource(dp);
			string[] array = default(string[]);
			int num2 = default(int);
			while (true)
			{
				CultureInfo culture = GetCulture(dp);
				int num;
				if (culture != null)
				{
					DebugInDesignTime(false, "正在尝试设置设计时区域属性……");
					num = 0;
					if (num == 0)
					{
						DebugInDesignTime((byte)num != 0, "区域：{0}", culture.Name);
						Lang.Current = culture;
						goto IL_0061;
					}
					goto IL_0062;
				}
				goto IL_0072;
				IL_00d9:
				array = new string[2] { "Lang.Language.DisplayName", "Lang.Operations.Main.Menu" };
				num2 = 0;
				goto IL_015d;
				IL_015d:
				while (true)
				{
					if (num2 >= array.Length)
					{
						return;
					}
					string text = array[num2];
					string value;
					int num3;
					if (Lang.TryGet(text, out value))
					{
						num3 = 0;
						if (num3 != 0)
						{
							goto IL_0159;
						}
						DebugInDesignTime((byte)num3 != 0, "测试 [{0}] {1}", text, value);
					}
					else
					{
						if (false)
						{
							break;
						}
						if (false)
						{
							goto IL_0061;
						}
						DebugInDesignTime(false, "测试 [{0}] 失败！", text);
					}
					num3 = num2;
					goto IL_0159;
					IL_0159:
					num2 = num3 + 1;
				}
				goto IL_00b8;
				IL_0072:
				if (!(source != null))
				{
					goto IL_00d9;
				}
				if (false)
				{
					continue;
				}
				DebugInDesignTime(false, "正在尝试读取设计时语言……");
				string text2 = Uri.UnescapeDataString(source.AbsolutePath);
				if (6 != 0)
				{
					DebugInDesignTime(false, "路径：{0}", text2);
					goto IL_00b8;
				}
				goto IL_015d;
				IL_0061:
				num = 0;
				goto IL_0062;
				IL_0062:
				DebugInDesignTime((byte)num != 0, "完成。");
				goto IL_0072;
				IL_00b8:
				Lang.Sources.Add(new XamlLanguageSource(text2));
				DebugInDesignTime(false, "完成。");
				goto IL_00d9;
			}
		}
	}
}
