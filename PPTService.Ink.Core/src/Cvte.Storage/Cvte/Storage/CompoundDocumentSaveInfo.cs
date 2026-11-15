using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace PPTService.Ink.Core.Storage
{
	
	
	[JsonObject(Id = "Document")]
	[SaveInfoContract("Document")]
	public class CompoundDocumentSaveInfo : SaveInfo
	{
		internal const string RelativeFileName = "Document";

		[SaveInfoMember("Name")]
		public string Name { get; set; }

		[SaveInfoMember("Creator")]
		public string Creator { get; set; }

		[SaveInfoMember("LastModifiedBy")]
		public string LastModifiedBy { get; set; }

		[JsonConverter(typeof(SafeIsoDateTimeConverter))]
		[SaveInfoMember("CreatedDateTime")]
		public DateTime CreatedDateTime { get; set; }

		[JsonConverter(typeof(SafeIsoDateTimeConverter))]
		[SaveInfoMember("ModifiedDateTime")]
		public DateTime ModifiedDateTime { get; set; }

		[SaveInfoMember("CreatedDocumentVersion")]
		public string CreatedDocumentVersion { get; set; }

		[JsonProperty("DocumentVersion")]
		[SaveInfoMember("DocumentVersion", IsRequired = true)]
		public string DocumentVersion { get; set; }

		[SaveInfoMember("CreatedAppVersion")]
		public string CreatedAppVersion { get; set; }

		[SaveInfoMember("AppVersion")]
		public string AppVersion { get; set; }
	}
}
