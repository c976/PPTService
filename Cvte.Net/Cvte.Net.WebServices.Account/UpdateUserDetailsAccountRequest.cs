using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Cvte.Net.Http;

namespace Cvte.Net.WebServices.Account
{
	
	
	[Request("api/v1/auth/users/info", RequestMethods.Put)]
	[DataContract]
	internal class UpdateUserDetailsAccountRequest : AccountHttpRequest
	{
		[DataMember(Name = "nickname")]
		public string Nickname { get; set; }

		[DataMember(Name = "realName")]
		public string RealName { get; set; }

		[DataMember(Name = "stageId")]
		public string StageId { get; set; }

		[DataMember(Name = "subjectId")]
		public string SubjectId { get; set; }

		[DataMember(Name = "photoUrl")]
		public string UserPhoto { get; set; }

		[DataMember(Name = "provinceId")]
		public string ProvinceId { get; set; }

		[DataMember(Name = "cityId")]
		public string CityId { get; set; }

		[DataMember(Name = "address")]
		public string Address { get; set; }

		[DataMember(Name = "gender")]
		public string Ggender { get; set; }

		[DataMember(Name = "idNumber")]
		public string IdNumber { get; set; }
	}
}
