using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	[Request("api/v1/auth/captchas")]
	[DataContract]
	internal class GetCaptchaRequest : AccountHttpRequest
	{
	}
}
