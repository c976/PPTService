using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account.Seewo.QrCode.Models
{
	
	
	[Request("scan/pcCheckQrcode")]
	[DataContract]
	internal class CheckQrCodeRequest : JsonRequest
	{
		[IgnoreDataMember]
		public string BarCode { get; set; }

		public CheckQrCodeRequest(string barCode)
		{
			BarCode = barCode;
		}
	}
}
