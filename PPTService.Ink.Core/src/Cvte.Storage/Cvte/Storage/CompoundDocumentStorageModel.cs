using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	public class CompoundDocumentStorageModel : StorageModel
	{
		[StorageInfoMember(Name = "Document")]
		public CompoundDocumentSaveInfo Document { get; internal set; }

		[StorageInfoMember(Name = "thumbnail.png", Type = StorageFileType.Thumbnail)]
		public string Thumbnail { get; set; }

		public CompoundDocumentStorageModel()
		{
			Document = new CompoundDocumentSaveInfo();
		}
	}
}
