using System.Runtime.Serialization;
using Cvte.Pipe.Http.Request;

namespace Cvte.WebService.Account
{
	[DataContract]
	internal class AccountGetRequestBase<TBody> : DynamicGetRequest where TBody : AccountRequestBody
	{
		[DataMember(Name = "data")]
		public TBody Body { get; protected set; }

		[DataMember(Name = "sign")]
		public string Sign { get; set; }

		public override string GetRequestUrl()
		{
			Sign = Body.GetSign();
			return SA_Proxy._0086___0018(this);
		}
	}
}
