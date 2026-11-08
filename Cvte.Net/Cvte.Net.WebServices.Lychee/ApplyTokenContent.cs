using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Lychee
{
	
	
	[DataContract]
	internal class ApplyTokenContent
	{
		
		[DataContract]
		public class Page
		{
			[DataMember(Order = 0, IsRequired = true, Name = "sortno")]
			public string SortNo { get; set; }

			[DataMember(Order = 1, IsRequired = true, Name = "name")]
			public string Name { get; set; }

			[DataMember(Order = 2, IsRequired = true, Name = "_id")]
			public string Id { get; set; }
		}

		[DataMember(Name = "pid")]
		public string Pid { get; set; }

		[DataMember(Name = "purl")]
		public string PUrl { get; set; }

		[DataMember(Name = "purlqr")]
		public string PUrlQr { get; set; }

		[DataMember(Name = "upurl")]
		public string UpUrl { get; set; }

		[DataMember(Name = "token")]
		public string Token { get; set; }

		[DataMember(Name = "appid")]
		public string AppId { get; set; }

		[DataMember(Name = "pages")]
		public List<Page> Pages { get; set; }
	}
}
