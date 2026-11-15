using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class CompoundDocument
	{
		public string Name { get; set; }

		public string Version { get; internal set; }

		public string Hash
		{
			get
			{
				IEnumerable<string> enumerable = from f in Files
					orderby f.Hash descending
					select f.Hash;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string item in enumerable)
				{
					stringBuilder.Append(item);
				}
				return Md5SecurityUtility.Encrypt32(stringBuilder.ToString());
			}
		}

		public long Size => Files.Sum( (FileItem f) => (!File.Exists(f.AbsolutePath)) ? 0 : new FileInfo(f.AbsolutePath).Length);

		public IList<FileItem> Files { get; }

		public CompoundDocument()
			: this(null)
		{
		}

		public CompoundDocument(string name, string version, IList<FileItem> files)
		{
			Name = name;
			Version = version;
			Files = files ?? new List<FileItem>();
		}

		public CompoundDocument(IList<FileItem> files)
		{
			Files = files ?? new List<FileItem>();
		}

		public bool IsExist(FileItem fileItem)
		{
			return Files.Any( (FileItem file) => file.RelativePath == fileItem.RelativePath && file.Hash == fileItem.Hash);
		}

		public bool IsExist(string absolutePath)
		{
			return Files.Any( (FileItem file) => file.AbsolutePath == absolutePath);
		}

		public void AddFile(FileItem file)
		{
			if (file != null)
			{
				Files.Add(file);
			}
		}

		public void AddFile(string absolutePath, string relativePath)
		{
			if (absolutePath != null && relativePath != null)
			{
				Files.Add(new FileItem(absolutePath, relativePath));
			}
		}

		public bool TryGetFile(string relativePath, out FileItem fileItem)
		{
			fileItem = Files.FirstOrDefault( (FileItem f) => PathComparisonHelper.IsSameRelativePath(f.RelativePath, relativePath));
			return fileItem != null;
		}

		public IList<FileItem> GetFiles(string relativeDir)
		{
			return Files.Where( (FileItem f) => PathComparisonHelper.IsStartWithRelativePath(f.RelativePath, relativeDir)).ToList();
		}

		public IList<FileItem> GetAllFiles()
		{
			return Files.ToList();
		}

		public void Clear()
		{
			Files.Clear();
		}
	}
}
