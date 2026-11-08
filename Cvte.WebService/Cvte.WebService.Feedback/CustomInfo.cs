using System.Runtime.Serialization;

namespace Cvte.WebService.Feedback
{
	[DataContract]
	public class CustomInfo
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "field")]
		public string Field { get; set; }

		[DataMember(Name = "value")]
		public string Value { get; set; }

		public CustomInfo(string field, string name, string value)
		{
			Field = field;
			Name = name;
			Value = value;
		}
	}
}
