using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	
	
	[DataContract]
	public class Stage
	{
		[DataMember(Name = "stageId")]
		public string StageId { get; set; }

		[DataMember(Name = "stageName")]
		public string StageName { get; set; }

		[DataMember(Name = "subjects")]
		public IList<Subject> Subjects { get; set; }

		public Stage(string id, string name)
		{
			StageId = id;
			StageName = name;
		}
	}
}
