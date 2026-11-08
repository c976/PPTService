using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Myou
{
	
	
	[DataContract]
	internal class ExceptionEntry
	{
		[DataMember(Name = "sid")]
		public string SessionId { get; set; }

		[DataMember(Name = "ts")]
		public long Time { get; set; }

		[DataMember(Name = "ep")]
		public IDictionary<string, string> ExceptionProperties { get; set; }

		[DataMember(Name = "cp")]
		public IDictionary<string, string> CommonProperties { get; set; }
	}
}
