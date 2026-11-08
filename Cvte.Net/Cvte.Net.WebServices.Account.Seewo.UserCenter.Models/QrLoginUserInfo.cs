using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account.Seewo.UserCenter.Models
{
	
	
	[DataContract]
	public class QrLoginUserInfo
	{
		[DataMember(Name = "resourceid")]
		public string ResourceId { get; set; }

		[DataMember(Name = "tokenId")]
		public string TokenId { get; set; }

		[DataMember(Name = "userName")]
		public string UserId { get; set; }

		[DataMember(Name = "cnName")]
		public string CnName { get; set; }

		[DataMember(Name = "photoUrl")]
		public string PhotoUrl { get; set; }

		[DataMember(Name = "phone")]
		public string Phone { get; set; }
	}
}
