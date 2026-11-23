using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
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
