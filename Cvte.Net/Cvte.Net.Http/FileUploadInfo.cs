using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.Http
{
	
	
	[DataContract]
	public class FileUploadInfo
	{
		[DataMember(Name = "appId")]
		public string AppId { get; set; }

		[DataMember(Name = "expireSeconds")]
		public int ExpireSeconds { get; set; }

		[DataMember(Name = "keyPrefix")]
		public string KeyPrefix { get; set; }

		[DataMember(Name = "policyList")]
		public IList<PolicyEntry> PolicyList { get; set; } = new List<PolicyEntry>();
	}
}
