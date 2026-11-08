using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	public class Subject
	{
		[DataMember(Name = "subjectId")]
		public string SubjectId { get; set; }

		[DataMember(Name = "subjectName")]
		public string SubjectName { get; set; }

		public Subject(string id, string name)
		{
			SubjectId = id;
			SubjectName = name;
		}
	}
}
