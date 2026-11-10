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
				return;
			}
			forceShow = Debugger.IsAttached && forceShow;
			if (forceShow || DesignerProperties.GetIsInDesignMode(new DependencyObject()))
			{
				if (_designTimeWindow == null || !_designTimeWindow.IsLoaded)
				{
					Window window = new Window();
					window.Title = "EasiNote 多语言设计时调试窗口";
					window.Width = 320.0;
					window.Height = 540.0;
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
				}
				((TextBox)_designTimeWindow.Content).Text += string.Format(text + "\r\n", args);
			}
		}

		public DesignTimeXamlLang()
		{
			DebugInDesignTime(true, "设计时语言模块初始化……");
		}

		private static void UpdateDesignTimeLanguage(DependencyObject dp)
		{
			Uri source = GetSource(dp);
			CultureInfo culture = GetCulture(dp);
			if (culture != null)
			{
				DebugInDesignTime(false, "正在尝试设置设计时区域属性……");
				DebugInDesignTime(false, "区域：{0}", culture.Name);
				Lang.Current = culture;
				DebugInDesignTime(false, "完成。");
			}
			if (source != null)
			{
				DebugInDesignTime(false, "正在尝试读取设计时语言……");
				string text = Uri.UnescapeDataString(source.AbsolutePath);
				DebugInDesignTime(false, "路径：{0}", text);
				Lang.Sources.Add(new XamlLanguageSource(text));
				DebugInDesignTime(false, "完成。");
			}
			string[] array = new string[2] { "Lang.Language.DisplayName", "Lang.Operations.Main.Menu" };
			foreach (string text2 in array)
			{
				string value;
				if (Lang.TryGet(text2, out value))
				{
					DebugInDesignTime(false, "测试 [{0}] {1}", text2, value);
				}
				else
				{
					DebugInDesignTime(false, "测试 [{0}] 失败！", text2);
				}
			}
		}
	}
}
