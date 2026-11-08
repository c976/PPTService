using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class UserDetails
	{
		[DataMember(Name = "uid")]
		public string UserId { get; set; }

		[DataMember(Name = "password")]
		public string Password { get; set; }

		[DataMember(Name = "username")]
		public string UserName { get; set; }

		[DataMember(Name = "nickname")]
		public string Nickname { get; set; }

		[DataMember(Name = "realName")]
		public string RealName { get; set; }

		[DataMember(Name = "photoUrl")]
		public string UserPhoto { get; set; }

		[DataMember(Name = "unitId")]
		public string UnitId { get; set; }

		[DataMember(Name = "wechatUid")]
		public string WechatUid { get; set; }

		[DataMember(Name = "idNumber")]
		public string IdNumber { get; set; }

		[DataMember(Name = "phone")]
		public string Phone { get; set; }

		[DataMember(Name = "email")]
		public string Email { get; set; }

		[DataMember(Name = "stageId")]
		public string StageId { get; set; }

		[DataMember(Name = "stageName")]
		public string StageName { get; set; }

		[DataMember(Name = "subjectId")]
		public string SubjectId { get; set; }

		[DataMember(Name = "subjectName")]
		public string SubjectName { get; set; }

		[DataMember(Name = "unitName")]
		public string UnitName { get; set; }

		[DataMember(Name = "provinceId")]
		public string ProvinceId { get; set; }

		[DataMember(Name = "cityId")]
		public string CityId { get; set; }

		[DataMember(Name = "address")]
		public string Address { get; set; }

		[DataMember(Name = "gender")]
		public string Ggender { get; set; }
	}
}
