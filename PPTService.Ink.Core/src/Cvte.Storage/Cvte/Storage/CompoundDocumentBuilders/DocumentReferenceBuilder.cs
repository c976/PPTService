using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.CompoundDocumentBuilders
{
	
	
	internal class DocumentReferenceBuilder : DocumentBuilderBase
	{
		public DocumentReferenceBuilder(CompoundDocument document, FileProvider fileProvider, ISaveInfoParser parser, string buildPath, CompoundDocumentLogger documentLogger)
			: base(document, fileProvider, parser, buildPath, documentLogger)
		{
		}

		public void AddRelationship(ReferenceProvider referenceProvider)
		{
			List<RelationshipSaveInfo> list = new List<RelationshipSaveInfo>();
			foreach (ReferenceInfo reference in referenceProvider.ReferenceManager.References)
			{
				string relativePath = referenceProvider.ReferenceManager.GetRelativePath(reference.ReferenceId);
				Document.AddFile(new FileItem(reference.AbsolutePath, relativePath, reference.Hash));
				list.Add(new RelationshipSaveInfo
				{
					Id = reference.ReferenceId,
					Target = relativePath,
					HashCode = reference.Hash
				});
			}
			if (list.Any())
			{
				string filePath = referenceProvider.ReferenceManager.FilePath;
				Document.AddFile(FileProvider.BuildFile(new ReferenceSaveInfo
				{
					Relationships = list
				}, Parser, BuildPath, filePath, DocumentLogger));
			}
		}
	}
}
