using System.Runtime.Serialization;

namespace Cvte.WebService.Account
{
	[DataContract]
	public class UserInfo
	{
		public string Key { get; set; }

		[DataMember(Name = "userName")]
		public string Id { get; set; }

		[DataMember(Name = "cnname")]
		public string Name { get; set; }

		[DataMember(Name = "photoURL")]
		public string Photo { get; set; }

		public string Password { get; set; }

		public string TokenId { get; set; }

		public long LastTime { get; set; }
	}
}
