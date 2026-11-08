using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Cvte.Windows.Localization
{
	public class XamlLanguageReader : ILanguageReader
	{
		private readonly IEnumerable<FileInfo> _files;

		public XamlLanguageReader(IEnumerable<string> fileNames)
		{
			_files = fileNames.Select((string x) => new FileInfo(x));
			if (_files.Any((FileInfo x) => !x.Exists))
			{
				throw new FileNotFoundException("指定的语言文件集合中包含不存在的文件。");
			}
		}

		public Dictionary<string, string> Read(CultureInfo culture)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(256);
			do
			{
				IEnumerator<KeyValuePair<string, string>> enumerator = _files.Where((FileInfo x) => x.Exists).SelectMany((FileInfo file) => ReadInner(file.FullName, culture)).GetEnumerator();
				try
				{
					if (false)
					{
						goto IL_007f;
					}
					goto IL_009a;
					IL_009a:
					if (enumerator.MoveNext())
					{
						goto IL_007f;
					}
					goto end_IL_007a;
					IL_007f:
					KeyValuePair<string, string> current = enumerator.Current;
					dictionary[current.Key] = current.Value;
					goto IL_009a;
					end_IL_007a:;
				}
				finally
				{
					while (enumerator != null)
					{
						if (7 != 0)
						{
							if (8 != 0)
							{
								enumerator.Dispose();
							}
							break;
						}
					}
				}
			}
			while (false);
			return dictionary;
		}

		private IEnumerable<KeyValuePair<string, string>> ReadInner(string fileName, CultureInfo culture)
		{
			if (uint.MaxValue != 0)
			{
			}
			CultureInfo culture2 = culture;
			FileInfo fileInfo = new FileInfo(fileName);
			FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			try
			{
				ResourceDictionary resourceDictionary;
				try
				{
					resourceDictionary = XamlReader.Load((Stream)fileStream) as ResourceDictionary;
				}
				catch (XamlParseException innerException)
				{
					throw new FileFormatException($"语言文件 {fileInfo.FullName} 不是一个有效的 xaml 文件。", innerException);
				}
				if (resourceDictionary == null)
				{
					return new Dictionary<string, string>();
				}
				return from pair in resourceDictionary.OfType<DictionaryEntry>()
					select new KeyValuePair<string, string>(pair.Key.ToString(), string.Format(culture2, "{0}", new object[1] { pair.Value }));
			}
			finally
			{
				if (5 == 0 || fileStream != null)
				{
					((IDisposable)fileStream).Dispose();
				}
			}
		}
	}
}
