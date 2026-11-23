using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;

namespace PPTServiceUltra.Ink.Core.Windows.Localization
{
	public class XamlLanguageSource : ILanguageSource
	{
		private const string FileExtension = ".xaml";

		private const string FilePattern = "*.xaml";

		public Uri Source { get; }

		public XamlLanguageSource(string folder)
		{
			Source = new Uri(new DirectoryInfo(folder).FullName, UriKind.Absolute);
		}

		public XamlLanguageSource(Uri source)
		{
			Source = source;
		}

		public IEnumerable<Language> GetLanguages()
		{
			if (Source.IsFile)
			{
				return LoadFromFileSystem();
			}
			return LoadFromResourceDictionary();
		}

		private IEnumerable<Language> LoadFromFileSystem()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Source.LocalPath);
			if (!directoryInfo.Exists)
			{
				return Enumerable.Empty<Language>();
			}
			DirectoryInfo[] array = directoryInfo.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).ToArray();
			FileInfo[] array2 = directoryInfo.EnumerateFiles("*.xaml", SearchOption.TopDirectoryOnly).ToArray();
			List<Language> list = new List<Language>(array.Length + array2.Length);
			list.AddRange(from x in array.Select((DirectoryInfo dir) =>
				{
					CultureInfo culture = GetCulture(dir.Name);
					return (culture != null) ? new Language(culture, new XamlLanguageReader(from x in dir.EnumerateFiles("*.xaml", SearchOption.AllDirectories)
						select x.FullName)) : null;
				})
				where x != null
				select x);
			list.AddRange(from x in array2.Select((FileInfo file) =>
				{
					CultureInfo culture = GetCulture(Path.GetFileNameWithoutExtension(file.Name));
					return (culture != null) ? new Language(culture, new XamlLanguageReader(new string[1] { file.FullName })) : null;
				})
				where x != null
				select x);
			return list;
		}

		private static CultureInfo GetCulture(string name)
		{
			CultureInfo result = null;
			try
			{
				result = new CultureInfo(name);
			}
			catch (CultureNotFoundException)
			{
			}
			return result;
		}

		private IEnumerable<Language> LoadFromResourceDictionary()
		{
			return from x in new ResourceDictionary
				{
					Source = Source
				}.MergedDictionaries.Select((ResourceDictionary x) =>
				{
					string[] array = x.Source.OriginalString.Split(new char[2] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
					return (array != null && array.Length == 2 && array[1].ToLower() == "xaml") ? new Language(new CultureInfo(array[0]), new ResourceDictionaryLanguageReader(x)) : null;
				})
				where x != null
				select x;
		}
	}
}
