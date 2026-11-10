using System.Runtime.Serialization;
using Cvte.Pipe.Http.Response;

namespace Cvte.WebService.Account
{
	[DataContract]
	public class AccountResponse : JsonResponse
	{
		[DataMember(Name = "statusCode")]
		public AccountResponseCode StatusCode { get; set; }

		[DataMember(Name = "errorMessage")]
		public string ErrorMessage { get; set; }

		public override bool Succeed
		{
			get
			{
				if (Html != null)
				{
					return StatusCode == AccountResponseCode.OK;
				}
				return false;
			}
		}

		public override int? MonitorCode => (int)StatusCode;
	}
}
