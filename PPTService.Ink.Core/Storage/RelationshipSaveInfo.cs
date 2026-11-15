using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	[SaveInfoContract("Relationship")]
	public class RelationshipSaveInfo : SaveInfo
	{
		[SaveInfoMember("Id", IsRequired = true)]
		public string Id { get; set; }

		[SaveInfoMember("Target", IsRequired = true)]
		public string Target { get; set; }

		[SaveInfoMember("Hash")]
		public string HashCode { get; set; }
	}
}
