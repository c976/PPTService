using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Cvte.Windows.Localization
{
	public class XamlLanguageSource : ILanguageSource
	{
		private const string FileExtension = ".xaml";

		private const string FilePattern = "*.xaml";

		public string FolderName { get; set; }

		public XamlLanguageSource(string folder)
		{
			FolderName = folder;
		}

		public IEnumerable<Language> GetLanguages()
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(FolderName);
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
	}
}
