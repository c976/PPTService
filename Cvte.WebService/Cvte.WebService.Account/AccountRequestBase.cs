using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class AccountRequestBase<TBody> : FormsRequest where TBody : AccountRequestBody
	{
		[DataMember(Name = "data")]
		public TBody Body { get; protected set; }

		[DataMember(Name = "sign")]
		public string Sign { get; set; }

		public override byte[] GetRequestData()
		{
			Sign = Body.GetSign();
			return base.GetRequestData();
		}
	}
}
