using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public abstract class CompoundDocumentFileAdapter : ICompoundDocumentAdapter<string>
	{
		protected abstract string VersionPath { get; }

		protected virtual string TempPath => Path.GetTempPath();

		public bool IsLegal(string target)
		{
			try
			{
				using (ZipArchive zipArchive = ZipFile.Open(target, ZipArchiveMode.Read))
				{
					return zipArchive.Entries.Any( (ZipArchiveEntry z) => z.FullName == VersionPath);
				}
			}
			catch (FileFormatException)
			{
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool TryGetVersion(string target, out string version)
		{
			try
			{
				using (ZipArchive zipArchive = ZipFile.Open(target, ZipArchiveMode.Read))
				{
					ZipArchiveEntry zipArchiveEntry = zipArchive.Entries.FirstOrDefault( (ZipArchiveEntry z) => z.FullName == VersionPath);
					if (zipArchiveEntry == null)
					{
						version = string.Empty;
						return false;
					}
					using (StreamReader streamReader = new StreamReader(zipArchiveEntry.Open()))
					{
						string text = streamReader.ReadToEnd();
						return GetVersion(text, out version);
					}
				}
			}
			catch (Exception)
			{
				version = string.Empty;
				return false;
			}
		}

		public bool TryLoad(string target, out CompoundDocument document)
		{
			try
			{
				string text = Path.Combine(TempPath, Guid.NewGuid().ToString());
				ZipPackage.DecompressFileCompatibly(target, text);
				List<FileItem> list = new List<FileItem>();
				LoadFiles(text, string.Empty, list);
				document = new CompoundDocument(list);
				string version;
				TryGetVersion(target, out version);
				document.Version = version;
				document.Name = Path.GetFileNameWithoutExtension(target);
				return true;
			}
			catch (Exception)
			{
				document = null;
				return false;
			}
		}

		public virtual void Save(CompoundDocument document, string target)
		{
			string text = Path.Combine(TempPath, Guid.NewGuid().ToString() + ".tmp");
			try
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
				ZipPackage.CompressFiles(text, document.Files.Select(  (FileItem f) => new ValueTuple<string, string>(f.AbsolutePath, f.RelativePath)));
				if (File.Exists(target))
				{
					File.Delete(target);
				}
				File.Move(text, target);
			}
			catch (Exception)
			{
				try
				{
					if (File.Exists(text))
					{
						File.Delete(text);
					}
				}
				catch (Exception)
				{
				}
				throw;
			}
		}

		public virtual void Save(CompoundDocument originDocument, string target, CompoundDocument newDocument)
		{
			if (string.IsNullOrEmpty(target))
			{
				throw new InvalidOperationException("无法对未保存的文档进行增量保存");
			}
			IList<FileItem> files = newDocument.Files;
			ZipPackage.CompressFiles(target, from f in files
				where !originDocument.IsExist(f)
				select new ValueTuple<string, string>(f.AbsolutePath, f.RelativePath));
			IList<string> list = (from file in originDocument.Files
				where newDocument.Files.All( (FileItem f) => f.RelativePath != file.RelativePath)
				select file.RelativePath).ToList();
			if (list.Contains(VersionPath))
			{
				list.Remove(VersionPath);
			}
			ZipPackage.DeleteFiles(target, list);
		}

		private static void LoadFiles(string absoluteDir, string relativeDir, ICollection<FileItem> resultFiles)
		{
			string[] files = Directory.GetFiles(absoluteDir);
			foreach (string text in files)
			{
				string fileName = Path.GetFileName(text);
				if (fileName != null)
				{
					resultFiles.Add(new FileItem(text, Path.Combine(relativeDir, fileName)));
				}
			}
			files = Directory.GetDirectories(absoluteDir);
			foreach (string text2 in files)
			{
				LoadFiles(text2, Path.Combine(relativeDir, new DirectoryInfo(text2).Name), resultFiles);
			}
		}

		protected abstract bool GetVersion(string text, out string version);
	}
}
