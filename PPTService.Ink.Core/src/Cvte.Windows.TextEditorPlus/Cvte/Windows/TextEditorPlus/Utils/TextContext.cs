using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using PPTService.Ink.Core.ComponentModel;
using PPTService.Ink.Core.Documents;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Windows.Localization;
using PPTService.Ink.Core.Windows.TextEditorPlus.Culture;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document;
using PPTService.Ink.Core.Windows.TextEditorPlus.Render;
using PPTService.Ink.Core.Windows.TextEditorPlus.Storage;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Utils
{
	public static class TextContext
	{
		private static List<string> _installedFontFamilies;

		private static List<string> _installedFontFamiliesEx;

		private static IUndoRedoProvider _undoRedoProvider;

		private static ISaveInfoParser _storageParser;

		private static ISaveInfoParser _defaultParser;

		private static SaveInfoManager _saveInfoManager;

		private static readonly IUndoRedoProvider DefaultUndoRedoProvider;

		private static readonly Dictionary<string, string> EscapedReplace;

		internal static readonly System.Windows.Media.FontFamily DefaultRenderFontFamily;

		internal static readonly List<string> NotInstalledFontFamilies;

		private static readonly Lazy<System.Windows.Media.Brush> _selectionBrush;

		public const double PoundToPx = 1.333333333;

		public const double PxToPound = 0.75;

		public const double MaxFontSize = 500.0;

		public const double MinFontSize = 1.0;

		public const double Epsilon = 1E-05;

		public const char UnkonwnChar = '\ufffd';

		public static readonly System.Windows.Media.FontFamily DefaultFontFamily;

		public static readonly Dictionary<MarkerStyle, string> MarkerStringDic;

		public const string NewLine = "\r\n";

		public static readonly string[] NewLines;

		private static string _tempFolder;

		private static WeakReference<HashSet<string>> _installedFontFamiliesHashSet;

		private static ISaveInfoParser DefaultParser
		{
			get
			{
				if (_defaultParser == null)
				{
					lock (LockerObject)
					{
						if (_defaultParser == null)
						{
							_defaultParser = new XmlCompoundDocumentSaveInfoParser(SaveInfoManager, new CompoundDocumentReferenceManager());
						}
					}
				}
				return _defaultParser;
			}
			set
			{
				_defaultParser = value;
			}
		}

		private static object LockerObject => EscapedReplace;

		private static List<string> ApplicationFontResourceList { get; set; }

		public static System.Windows.Media.Brush DefaultSelectionBrush => _selectionBrush.Value;

		public static System.Windows.Media.Brush DefaultForeground => System.Windows.Media.Brushes.Black;

		public static System.Windows.Media.Brush DefaultBackground => System.Windows.Media.Brushes.Transparent;

		[Obsolete("请使用 TextContext.NewLines 代替")]
		public static List<string> Newlines => NewLines.ToList();

		public static SaveInfoManager SaveInfoManager
		{
			get
			{
				if (_saveInfoManager == null)
				{
					lock (LockerObject)
					{
						if (_saveInfoManager == null)
						{
							SaveInfoManager = new SaveInfoManager();
						}
					}
				}
				return _saveInfoManager;
			}
			set
			{
				_saveInfoManager = value.LoadTextEditorAssembly();
			}
		}

		public static string TempFolder
		{
			get
			{
				if (string.IsNullOrEmpty(_tempFolder))
				{
					_tempFolder = DefaultTempFolder;
				}
				return _tempFolder;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("value");
				}
				_tempFolder = value;
			}
		}

		private static string DefaultTempFolder => Path.GetTempPath();

		public static ISaveInfoParser StorageParser
		{
			get
			{
				return _storageParser ?? DefaultParser;
			}
			set
			{
				_storageParser = value;
			}
		}

		public static IUndoRedoProvider UndoRedo
		{
			get
			{
				return _undoRedoProvider ?? DefaultUndoRedoProvider;
			}
			set
			{
				_undoRedoProvider = value;
			}
		}

		public static List<string> InstalledFontFamilies => _installedFontFamilies ?? (_installedFontFamilies = GetInstalledFamilies());

		[Obsolete("判断字体安装使用 CheckFontFamilyInstalled 方法，此方法内容和这个属性相同，只是使用 HASH 性能比较好")]
		public static List<string> InstalledFontFamiliesEx => _installedFontFamiliesEx ?? (_installedFontFamiliesEx = GetInstalledFamiliesEx());

		private static HashSet<string> InstalledFontFamiliesHashSet
		{
			get
			{
				HashSet<string> target = null;
				_installedFontFamiliesHashSet?.TryGetTarget(out target);
				if (target == null)
				{
					target = new HashSet<string>(InstalledFontFamiliesEx);
					_installedFontFamiliesHashSet = new WeakReference<HashSet<string>>(target);
				}
				return target;
			}
		}

		public static ITextCharacterSizeBuilderProvider TextCharacterSizeBuilderProvider { get; set; }

		static TextContext()
		{
			EscapedReplace = new Dictionary<string, string>
			{
				{ "\v", "\r\n" },
				{ "\a", "" },
				{ "\b", "" },
				{ "\f", "" },
				{ "\0", "" }
			};
			DefaultRenderFontFamily = new System.Windows.Media.FontFamily("Arial");
			NotInstalledFontFamilies = new List<string>();
			_selectionBrush = new Lazy<System.Windows.Media.Brush>(() =>
			{
				SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.SystemColors.HighlightColor);
				solidColorBrush.Opacity = 0.5;
				solidColorBrush.Freeze();
				return solidColorBrush;
			});
			DefaultFontFamily = new System.Windows.Media.FontFamily("微软雅黑");
			MarkerStringDic = new Dictionary<MarkerStyle, string>
			{
				{
					MarkerStyle.Rect,
					"■"
				},
				{
					MarkerStyle.Box,
					"▶"
				},
				{
					MarkerStyle.Circle,
					"●"
				},
				{
					MarkerStyle.Decimal,
					"1"
				},
				{
					MarkerStyle.Decimal1,
					"(1)"
				},
				{
					MarkerStyle.Decimal2,
					"①"
				},
				{
					MarkerStyle.Tick,
					"√"
				},
				{
					MarkerStyle.UpperLatin,
					"A"
				},
				{
					MarkerStyle.Square,
					"★"
				},
				{
					MarkerStyle.LowerLatin,
					"a"
				}
			};
			NewLines = new string[3] { "\r\n", "\r", "\n" };
			DefaultUndoRedoProvider = new UndoRedoProvider();
			AddLanguage();
		}

		private static void AddLanguage()
		{
			try
			{
				Lang.Sources.Add(new LanguageSource());
			}
			catch (Exception exception)
			{
				TextTracer.Error(exception, "加载多语言");
			}
		}

		private static string EnsureDirectoryExisting(string directory)
		{
			if (!Directory.Exists(directory))
			{
				try
				{
					Directory.CreateDirectory(directory);
				}
				catch (IOException)
				{
				}
			}
			return directory;
		}

		private static List<string> FontFamiliesSortByZh(List<string> familyNames)
		{
			if (CultureInfo.CurrentCulture.Name.StartsWith("zh"))
			{
				int num = familyNames.FindLastIndex((string x) => char.IsLower(x[0]) || char.IsUpper(x[0])) + 1;
				if (num < familyNames.Count)
				{
					List<string> list = familyNames.Skip(num).ToList();
					list.AddRange(familyNames.Take(num));
					familyNames = list;
				}
			}
			return familyNames;
		}

		internal static void UpdateNotInstalledFontFamilies(RichTextProperty property)
		{
			foreach (RunProperty runProperty in property.RunProperties)
			{
				if (!CheckFontFamilyInstalled(runProperty.FontFamily.ToString()))
				{
					NotInstalledFontFamilies.Add(runProperty.FontFamily.ToString());
				}
			}
			foreach (ParagraphProperty paragraphProperty in property.ParagraphProperties)
			{
				if (!CheckFontFamilyInstalled(paragraphProperty.DefaultRunProperty.FontFamily.ToString()))
				{
					NotInstalledFontFamilies.Add(paragraphProperty.DefaultRunProperty.FontFamily.ToString());
				}
				if (paragraphProperty.MarkerProperty.FontFamily != null && !CheckFontFamilyInstalled(paragraphProperty.MarkerProperty.FontFamily.ToString()))
				{
					NotInstalledFontFamilies.Add(paragraphProperty.MarkerProperty.FontFamily.ToString());
				}
			}
		}

		private static List<string> GetSystemFontFamilies()
		{
			try
			{
				CultureInfo currentCulture = CultureInfo.CurrentUICulture;
				CultureInfo defaultCulture = new CultureInfo("en-US");
				List<string> list = (from x in Fonts.SystemFontFamilies
					select x.FamilyNames into x
					select x.FirstOrDefault((KeyValuePair<XmlLanguage, string> k) => object.Equals(k.Key.GetSpecificCulture(), currentCulture)) into x
					where x.Key != null
					select x.Value).ToList();
				list.Sort();
				if (!object.Equals(currentCulture, defaultCulture))
				{
					List<string> list2 = (from x in Fonts.SystemFontFamilies
						select x.FamilyNames into x
						where x.Any((KeyValuePair<XmlLanguage, string> k) => object.Equals(k.Key.GetSpecificCulture(), defaultCulture)) && x.All((KeyValuePair<XmlLanguage, string> k) => !object.Equals(k.Key.GetSpecificCulture(), currentCulture))
						select x.FirstOrDefault((KeyValuePair<XmlLanguage, string> k) => object.Equals(k.Key.GetSpecificCulture(), defaultCulture)) into x
						where x.Key != null
						select x.Value).ToList();
					list2.Sort();
					list.AddRange(list2);
				}
				return list.ToList();
			}
			catch (Exception exception)
			{
				TextTracer.Error(exception);
				return new List<string>();
			}
		}

		internal static List<string> GetInstalledFamilies()
		{
			List<string> list = (from x in GetInstalledFontFamilies()
				where !string.IsNullOrEmpty(x.Name)
				select x.Name).ToList();
			list.AddRange(GetSystemFontFamilies());
			List<string> list2 = list.Distinct().ToList();
			list2.Sort();
			return FontFamiliesSortByZh(list2);
		}

		private static List<System.Drawing.FontFamily> GetInstalledFontFamilies()
		{
			try
			{
				return new InstalledFontCollection().Families.ToList();
			}
			catch (Exception exception)
			{
				TextTracer.Error(exception);
				return new List<System.Drawing.FontFamily>();
			}
		}

		public static void RegisterFontResource(IReadOnlyList<string> fontResourceUriList)
		{
			if (_installedFontFamiliesEx != null)
			{
				throw new InvalidOperationException("必须在文本初始化之前注册，请在 InstalledFontFamiliesEx 属性和 CheckFontFamilyInstalled 调用之前注册");
			}
			if (DebuggingProperties.IsDebug)
			{
				foreach (string fontResourceUri in fontResourceUriList)
				{
					if (!fontResourceUri.StartsWith("pack://", StringComparison.OrdinalIgnoreCase))
					{
						throw new ArgumentException("传入的字体应该是嵌入资源，如 pack://application:,,,/TextEditor;component/#Font 实际传入的是" + fontResourceUri, "fontResourceUriList");
					}
				}
			}
			if (ApplicationFontResourceList == null)
			{
				ApplicationFontResourceList = new List<string>();
			}
			ApplicationFontResourceList.AddRange(fontResourceUriList);
		}

		private static List<string> GetInstalledFamiliesEx()
		{
			List<System.Drawing.FontFamily> installedFontFamilies = GetInstalledFontFamilies();
			List<string> list = (from x in installedFontFamilies
				select x.Name into x
				where !string.IsNullOrEmpty(x)
				select x).ToList();
			foreach (string item in from x in installedFontFamilies
				select x.GetName(1033) into x
				where !string.IsNullOrEmpty(x)
				select x)
			{
				if (!list.Contains(item))
				{
					list.Add(item);
				}
			}
			if (ApplicationFontResourceList != null)
			{
				list.AddRange(ApplicationFontResourceList);
				ApplicationFontResourceList.Clear();
				ApplicationFontResourceList = null;
			}
			List<string> list2 = new List<string>();
			list2.AddRange(list);
			foreach (string item2 in list)
			{
				try
				{
					System.Windows.Media.FontFamily fontFamily = new System.Windows.Media.FontFamily(item2);
					list2.AddRange(fontFamily.FamilyNames.Values.ToList());
				}
				catch (Exception exception)
				{
					TextTracer.Warning(exception, "获取当前机器上的字体名称");
				}
			}
			list2.AddRange(GetSystemFontFamilies());
			return list2.Distinct().ToList();
		}

		public static bool CheckFontFamilyInstalled(string fontFamilySource)
		{
			return InstalledFontFamiliesHashSet.Contains(fontFamilySource);
		}

		public static string IllegalWordFilter(string text)
		{
			if (text == null)
			{
				return string.Empty;
			}
			foreach (string key in EscapedReplace.Keys)
			{
				if (text.Contains(key))
				{
					text = text.Replace(key, EscapedReplace[key]);
				}
			}
			return Regex.Replace(text, "[\\x00-\\x08]|[\\x0B-\\x0C]|[\\x0E-\\x1F]", "");
		}

		public static string ConvertToNormalText(string text)
		{
			return Regex.Replace(text, '\ufffd'.ToString(), string.Empty);
		}
	}
}
