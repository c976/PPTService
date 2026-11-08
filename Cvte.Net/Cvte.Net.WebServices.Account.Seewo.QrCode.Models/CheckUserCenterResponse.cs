using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.WebServices.Account.Seewo.UserCenter.Models;

namespace Cvte.Net.WebServices.Account.Seewo.QrCode.Models
{
	
	
	[DataContract]
	internal class CheckUserCenterResponse : UserCenterResponse
	{
		[DataMember(Name = "result")]
		public string Result { get; set; }

		[DataMember(Name = "token")]
		public string Token { get; set; }

		[DataMember(Name = "userName")]
		public string UserName { get; set; }
	}
}
