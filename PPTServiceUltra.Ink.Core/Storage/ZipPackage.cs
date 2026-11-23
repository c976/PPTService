using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal static class ZipPackage
	{
		private const string ZipContentTypesFileName = "[Content_Types].xml";

		public static void DeleteFiles(string target, IList<string> filesToDelete)
		{
			using (Package package = Package.Open(target, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
			{
				foreach (string item in filesToDelete)
				{
					Uri partUri = PackUriHelper.CreatePartUri(new Uri(".\\" + item, UriKind.Relative));
					if (package.PartExists(partUri))
					{
						package.DeletePart(partUri);
					}
				}
			}
		}

		public static void CompressFiles(string target,  IEnumerable<ValueTuple<string, string>> filesToCompress)
		{
			try
			{
				using (Package package = Package.Open(target, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
				{
					foreach (var item3 in filesToCompress)
					{
						string item = item3.Item1;
						string item2 = item3.Item2;
						if (item2.Trim().Equals("[Content_Types].xml", StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						Uri partUri = PackUriHelper.CreatePartUri(new Uri(".\\" + item2, UriKind.Relative));
						if (package.PartExists(partUri))
						{
							package.DeletePart(partUri);
						}
						PackagePart packagePart = package.CreatePart(partUri, "", CompressionOption.NotCompressed);
						using (FileStream fileStream = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							using (Stream destination = packagePart.GetStream())
							{
								fileStream.CopyTo(destination, 8192);
							}
						}
					}
				}
			}
			catch (Exception)
			{
				try
				{
					if (File.Exists(target))
					{
						File.Delete(target);
					}
				}
				catch (IOException)
				{
				}
				throw;
			}
		}

		public static IEnumerable<string> DecompressFile(string target, string outPath)
		{
			List<string> list = new List<string>();
			using (Package package = Package.Open(target, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				foreach (PackagePart part in package.GetParts())
				{
					if (part.Uri == null)
					{
						throw new InvalidOperationException("One Uri of Zip Parts is Null");
					}
					string text = Path.Combine(outPath, part.Uri.OriginalString.Substring(1));
					list.Add(text);
					string directoryName = Path.GetDirectoryName(text);
					if (!Directory.Exists(directoryName))
					{
						Directory.CreateDirectory(directoryName);
					}
					using (FileStream destination = new FileStream(text, FileMode.Create))
					{
						using (Stream stream = part.GetStream())
						{
							stream.CopyTo(destination);
						}
					}
				}
				return list;
			}
		}

		public static IEnumerable<string> DecompressFileCompatibly(string target, string outPath)
		{
			ZipArchive zipArchive = ZipFile.Open(target, ZipArchiveMode.Read);
			List<string> list = zipArchive.Entries.Select( (ZipArchiveEntry e) => e.FullName).ToList();
			zipArchive.ExtractToDirectory(outPath);
			list.Remove("[Content_Types].xml");
			return (from f in list
				where File.Exists(Path.Combine(outPath, f))
				select Path.Combine(outPath, f)).ToList();
		}
	}
}
