using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cvte.Net.WebServices.Files.Utils
{
	
	
	internal static class SpliceFileHelper
	{
		public static async Task<string> SpliceFiles(DirectoryInfo dir, string filePath)
		{
			string spliceFilePath = filePath + Path.GetRandomFileName() + ".tmp";
			string pattern = CommonFileHelper.GetFileNamePattern(filePath);
			IEnumerable<FileInfo> enumerable = from x in dir.GetFiles()
				where Regex.IsMatch(x.Name, pattern)
				select x;
			IList<FileInfo> fileInfos = (enumerable as IList<FileInfo>) ?? enumerable.ToList();
			if (fileInfos.Count > 1)
			{
				await Task.Run(() =>
				{
					IOrderedEnumerable<FileInfo> orderedEnumerable = fileInfos.OrderBy( (FileInfo file) => file.CreationTime);
					using (FileStream fileStream = new FileStream(spliceFilePath, FileMode.Append, FileAccess.Write))
					{
						foreach (FileInfo item in orderedEnumerable)
						{
							FileStream fileStream2 = File.OpenRead(item.FullName);
							fileStream2.CopyTo(fileStream);
							fileStream2.Close();
							File.Delete(item.FullName);
							fileStream.Position = fileStream.Length;
						}
					}
				}).ConfigureAwait(false);
			}
			else if (fileInfos.Count == 1)
			{
				return fileInfos[0].FullName;
			}
			return spliceFilePath;
		}

		public static async Task<long> GetDownloadedSize(DirectoryInfo directoryInfo, string filePath)
		{
			long result = 0L;
			string pattern = CommonFileHelper.GetFileNamePattern(filePath);
			List<FileInfo> list = (from x in directoryInfo.GetFiles()
				where Regex.IsMatch(x.Name, pattern)
				select x).ToList();
			if (list.Any())
			{
				string fileName = ((list.Count <= 1) ? list.ElementAt(0).FullName : (await SpliceFiles(directoryInfo, filePath)));
				result = new FileInfo(fileName).Length;
			}
			return result;
		}
	}
}
