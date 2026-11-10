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
			foreach (KeyValuePair<string, string> item in _files.Where((FileInfo x) => x.Exists).SelectMany((FileInfo file) => ReadInner(file.FullName, culture)))
			{
				dictionary[item.Key] = item.Value;
			}
			return dictionary;
		}

		private IEnumerable<KeyValuePair<string, string>> ReadInner(string fileName, CultureInfo culture)
		{
			FileInfo fileInfo = new FileInfo(fileName);
			using (FileStream stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				ResourceDictionary resourceDictionary;
				try
				{
					resourceDictionary = XamlReader.Load((Stream)stream) as ResourceDictionary;
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
					select new KeyValuePair<string, string>(pair.Key.ToString(), string.Format(culture, "{0}", new object[1] { pair.Value }));
			}
		}
	}
}
