using System.Runtime.Serialization;

namespace Cvte.Net.WebServices.Account
{
	[DataContract]
	public class CheckUserExistenceData
	{
		[DataMember(Name = "isUsed")]
		public bool IsUsed { get; set; }
	}
}
