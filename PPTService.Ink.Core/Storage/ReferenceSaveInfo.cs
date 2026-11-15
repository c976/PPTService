using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	[SaveInfoContract("Reference")]
	public class ReferenceSaveInfo : SaveInfo
	{
		[SaveInfoMember("Relationships", IsRequired = true)]
		public IList<RelationshipSaveInfo> Relationships { get; set; }

		public ReferenceSaveInfo()
		{
			Relationships = new List<RelationshipSaveInfo>();
		}
	}
}
