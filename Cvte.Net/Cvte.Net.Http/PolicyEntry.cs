using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.Http
{
	
	
	[DataContract]
	public class PolicyEntry
	{
		[DataMember(Name = "uploadUrl")]
		public string UploadUrl { get; set; }

		[DataMember(Name = "priority")]
		public int Priority { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "formFields")]
		public IList<FieldEntry> FormFields { get; set; } = new List<FieldEntry>();

		[DataMember(Name = "headerFields")]
		public IList<FieldEntry> HeaderFields { get; set; } = new List<FieldEntry>();
	}
}
