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
			DirectoryInfo directoryInfo2 = default(DirectoryInfo);
			if (0 == 0)
			{
				directoryInfo2 = directoryInfo;
			}
			if (!directoryInfo2.Exists)
			{
				return Enumerable.Empty<Language>();
			}
			DirectoryInfo[] array = directoryInfo2.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).ToArray();
			FileInfo[] array2 = directoryInfo2.EnumerateFiles("*.xaml", SearchOption.TopDirectoryOnly).ToArray();
			int num = array.Length;
			if (0 == 0)
			{
				num += array2.Length;
			}
			List<Language> list = new List<Language>(num);
			list.AddRange(from x in array.Select((DirectoryInfo dir) =>
				{
					CultureInfo culture = GetCulture(dir.Name);
					return (culture != null && 6 != 0) ? new Language(culture, new XamlLanguageReader(from x in dir.EnumerateFiles("*.xaml", SearchOption.AllDirectories)
						select x.FullName)) : null;
				})
				where x != null
				select x);
			list.AddRange(from x in array2.Select((FileInfo file) =>
				{
					CultureInfo culture;
					do
					{
						culture = GetCulture(Path.GetFileNameWithoutExtension(file.Name));
					}
					while (false);
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
