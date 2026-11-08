using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account.CVTouch
{
	[Request("/api/v2/captcha/prepare")]
	[DataContract]
	internal class GenerateCheckCodeHttpRequest : AccountHttpRequest
	{
	}
}
