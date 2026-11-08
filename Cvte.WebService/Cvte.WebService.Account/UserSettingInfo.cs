using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	public class UserSettingInfo
	{
		[DataMember(Name = "cnName")]
		public string UserName { get; set; }

		[DataMember(Name = "stageId")]
		public string Stage { get; set; }

		[DataMember(Name = "stageName")]
		public string StageName { get; set; }

		[DataMember(Name = "subjectId")]
		public string Subject { get; set; }

		[DataMember(Name = "subjectName")]
		public string SubjectName { get; set; }
	}
}
