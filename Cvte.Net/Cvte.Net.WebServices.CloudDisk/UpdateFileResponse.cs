using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[DataContract]
	public class UpdateFileResponse : CloudDiskHttpResponse
	{
		[DataMember(Name = "name")]
		public string Name { get; internal set; }

		[DataMember(Name = "size")]
		public long Size { get; internal set; }

		[DataMember(Name = "createdAt")]
		public long CreateTimeStamp { get; internal set; }

		[DataMember(Name = "updatedAt")]
		public long UpdateTimeStamp { get; internal set; }

		[DataMember(Name = "parentUid")]
		public string DirectoryId { get; set; }

		[DataMember(Name = "type")]
		public int TypeCode { get; internal set; }

		[DataMember(Name = "state")]
		public int StateCode { get; internal set; }

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
	}
}
