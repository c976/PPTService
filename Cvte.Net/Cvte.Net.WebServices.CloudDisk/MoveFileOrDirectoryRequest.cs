using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.CloudDisk
{
	
	
	[Request("/api/v1/directories/move", RequestMethods.Put)]
	[DataContract]
	internal class MoveFileOrDirectoryRequest : CloudDiskHttpRequest
	{
		[DataMember(Name = "src")]
		public IList<string> Sources { get; internal set; }

		[DataMember(Name = "target")]
		public string Target { get; internal set; }
	}
}
