using System.Runtime.Serialization;

namespace PPTService.Core.WebService.Feedback
{
	[DataContract]
	internal class FileInfo
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "url")]
		public string Url { get; set; }

		public FileInfo(string name, string url)
		{
			Name = name;
			Url = url;
		}
	}
}
