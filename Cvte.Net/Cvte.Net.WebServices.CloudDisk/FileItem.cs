using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[DataContract]
	public class FileItem
	{
		[DataMember(Name = "name")]
		public string FileName { get; set; }

		[DataMember(Name = "hash")]
		public string Hash { get; set; }

		[DataMember(Name = "size")]
		public long Size { get; set; }

		[DataMember(Name = "uid")]
		public string Uid { get; set; }

		[DataMember(Name = "createdAt")]
		public long CreateTimeStamp { get; internal set; }

		[DataMember(Name = "updatedAt")]
		public long UpdateTimeStamp { get; internal set; }

		[DataMember(Name = "storageKey")]
		internal string KeyValue { get; set; }

		[DataMember(Name = "type")]
		public int TypeCode { get; internal set; }

		[DataMember(Name = "state")]
		public int StateCode { get; internal set; }

		[DataMember(Name = "message")]
		public string Message { get; internal set; }

		public FileState State
		{
			get
			{
				FileState result;
				if (!Enum.TryParse<FileState>(StateCode.ToString(), out result))
				{
					return FileState.Unknow;
				}
				return result;
			}
		}

		public FileItem()
		{
		}

		internal FileItem(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("file not found", filePath);
			}
			FileName = Path.GetFileName(filePath);
			Size = new FileInfo(filePath).Length;
			Hash = SecurityUtility.GetMd5HashFromFile(filePath);
		}
	}
}
