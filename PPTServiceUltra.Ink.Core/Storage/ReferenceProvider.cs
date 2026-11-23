using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class ReferenceProvider
	{
		internal IReferenceManager ReferenceManager { get; }

		internal ReferenceProvider(IReferenceManager referenceManager)
		{
			ReferenceManager = referenceManager;
		}

		internal void LoadReferenceNode(StorableNode rootNode, ISaveInfoNodeParser nodeParser, IList<ReferenceInfo> referenceInfos)
		{
			List<RelationshipSaveInfo> list = new List<RelationshipSaveInfo>();
			foreach (ReferenceInfo referenceInfo in referenceInfos)
			{
				string relativePath = ReferenceManager.GetRelativePath(referenceInfo.ReferenceId);
				StorableNode node = new StorableNode
				{
					Value = referenceInfo.Hash,
					RelativePath = relativePath,
					InternalKey = relativePath,
					AbsolutePath = referenceInfo.AbsolutePath
				};
				rootNode.AddChild(node);
				list.Add(new RelationshipSaveInfo
				{
					Id = referenceInfo.ReferenceId,
					Target = relativePath,
					HashCode = referenceInfo.Hash
				});
			}
			if (list.Any())
			{
				string filePath = ReferenceManager.FilePath;
				StorableNode storableNode = nodeParser.DeparseNode(new ReferenceSaveInfo
				{
					Relationships = list
				});
				storableNode.InternalKey = filePath;
				storableNode.RelativePath = filePath;
				rootNode.AddChild(storableNode);
			}
		}

		internal IList<ReferenceInfo> ReadReferences(CompoundDocument document, ISaveInfoParser parser, FileProvider fileProvider)
		{
			string filePath = ReferenceManager.FilePath;
			if (string.IsNullOrEmpty(filePath))
			{
				return new List<ReferenceInfo>();
			}
			FileItem fileItem;
			if (!document.TryGetFile(filePath + ".xml", out fileItem))
			{
				return (from r in document.GetFiles(ReferenceManager.TargetPath)
					select new ReferenceInfo
					{
						ReferenceId = Path.GetFileName(r.RelativePath),
						AbsolutePath = r.AbsolutePath,
						Hash = r.Hash,
						Counter = 0
					}).ToList();
			}
			ReferenceSaveInfo referenceSaveInfo = parser.Parse(fileProvider.ReadContentFromFile(fileItem)) as ReferenceSaveInfo;
			if (referenceSaveInfo == null)
			{
				return new List<ReferenceInfo>();
			}
			List<ReferenceInfo> list = new List<ReferenceInfo>();
			if (referenceSaveInfo.Relationships == null)
			{
				referenceSaveInfo.Relationships = new List<RelationshipSaveInfo>();
				return list;
			}
			foreach (RelationshipSaveInfo relationship in referenceSaveInfo.Relationships)
			{
				FileItem fileItem2;
				if (document.TryGetFile(relationship.Target, out fileItem2))
				{
					list.Add(new ReferenceInfo
					{
						ReferenceId = relationship.Id,
						AbsolutePath = fileItem2.AbsolutePath,
						Hash = fileItem2.Hash,
						Counter = 0
					});
				}
			}
			return list;
		}
	}
}
