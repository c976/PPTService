using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
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
