using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PPTService.Core.WebService.Lychee
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

		[DataMember(Order = 0, IsRequired = true, Name = "pid")]
		public string Pid { get; set; }

		[DataMember(Order = 1, IsRequired = true, Name = "purl")]
		public string PUrl { get; set; }

		[DataMember(Order = 2, IsRequired = true, Name = "purlqr")]
		public string PUrlQr { get; set; }

		[DataMember(Order = 3, IsRequired = true, Name = "upurl")]
		public string UpUrl { get; set; }

		[DataMember(Order = 4, IsRequired = true, Name = "token")]
		public string Token { get; set; }

		[DataMember(Order = 5, IsRequired = true, Name = "appid")]
		public string AppId { get; set; }

		[DataMember(Order = 6, IsRequired = true, Name = "pages")]
		public List<Page> Pages { get; set; }

		[DataMember(Order = 7, IsRequired = true, Name = "uuid")]
		public string Uuid { get; set; }
	}
}
