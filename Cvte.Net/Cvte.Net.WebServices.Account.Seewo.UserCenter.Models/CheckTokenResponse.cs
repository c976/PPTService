using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account.Seewo.UserCenter.Models
{
	
	
	[DataContract]
	public class CheckTokenResponse : UserCenterResponse
	{
		[DataMember(Name = "UserInfo")]
		public QrLoginUserInfo UserInfo { get; set; }
	}
}
