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
				while (true)
				{
					if (1 == 0 || ((SA_Proxy._0083___0018(this) != null) ? true : false))
					{
						if (0 == 0)
						{
							break;
						}
					}
					else if (8 != 0)
					{
						return false;
					}
				}
				return StatusCode == AccountResponseCode.OK;
			}
		}

		public override int? MonitorCode => (int)StatusCode;
	}
}
