using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.BlockFile
{
	
	
	[DataContract]
	public class HeaderFileInfo
	{
		[DataMember(Name = "date")]
		public string Date { get; set; }

		[DataMember(Name = "authorization")]
		public string Authorization { get; set; }
	}
}
