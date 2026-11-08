using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account.Seewo.QrCode.Models
{
	[Request("scan/confirmStatus")]
	[DataContract]
	internal class ConfirmStatusRequest : JsonRequest
	{
	}
}
