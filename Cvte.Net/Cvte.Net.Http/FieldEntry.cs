using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.Http
{
	
	
	[DataContract]
	public class FieldEntry
	{
		[DataMember(Name = "key")]
		public string Key { get; set; }

		[DataMember(Name = "value")]
		public string Value { get; set; }
	}
}
