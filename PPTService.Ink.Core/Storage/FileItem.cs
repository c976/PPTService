using System.IO;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class FileItem
	{
		public string AbsolutePath { get; private set; }

		public string RelativePath { get; private set; }

		public string Hash { get; }

		public FileItem(string absolutePath, string relativePath)
			: this(absolutePath, relativePath, null)
		{
		}

		public FileItem(string absolutePath, string relativePath, string hash)
		{
			AbsolutePath = absolutePath;
			RelativePath = relativePath;
			if (string.IsNullOrEmpty(hash))
			{
				Hash = (File.Exists(absolutePath) ? Md5SecurityUtility.GetMd5HashFromFile(absolutePath) : string.Empty);
			}
			else
			{
				Hash = hash;
			}
		}

		private bool Equals(FileItem fileItem)
		{
			if (fileItem != null)
			{
				return fileItem.Hash == Hash;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			FileItem fileItem = obj as FileItem;
			if (fileItem != null)
			{
				return Equals(fileItem);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (Hash ?? string.Empty).GetHashCode();
		}
	}
}
