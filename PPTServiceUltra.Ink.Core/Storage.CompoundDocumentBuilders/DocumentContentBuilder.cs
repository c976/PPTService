using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.CompoundDocumentBuilders
{
	
	
	internal class DocumentContentBuilder : DocumentBuilderBase
	{
		public DocumentContentBuilder(CompoundDocument document, FileProvider fileProvider, ISaveInfoParser parser, string buildPath, CompoundDocumentLogger documentLogger)
			: base(document, fileProvider, parser, buildPath, documentLogger)
		{
		}

		public void AddContentFromStorageModel(CompoundDocumentStorageModel model)
		{
			InitContentBuilder();
			PropertyInfo[] contentMembers = model.GetContentMembers();
			foreach (PropertyInfo obj in contentMembers)
			{
				string storageInfoMemberName = obj.GetStorageInfoMemberName();
				string storageInfoMemberPath = obj.GetStorageInfoMemberPath();
				object value = obj.GetValue(model, null);
				SaveInfo saveInfo = value as SaveInfo;
				if (saveInfo != null)
				{
					string relativePath = Path.Combine(storageInfoMemberPath, storageInfoMemberName);
					Document.AddFile(FileProvider.BuildFile(saveInfo, Parser, BuildPath, relativePath, DocumentLogger));
					continue;
				}
				IList list = value as IList;
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						string relativePath2 = Path.Combine(storageInfoMemberPath, string.Format(storageInfoMemberName, j));
						Document.AddFile(FileProvider.BuildFile(list[j] as SaveInfo, Parser, BuildPath, relativePath2, DocumentLogger));
					}
				}
			}
		}

		public void AddContentFromStorableNode(StorableNode node, ISaveInfoNodeParser nodeParser)
		{
			InitContentBuilder();
			foreach (StorableNode item in node.Children.Where( (StorableNode storableNode) => !string.IsNullOrWhiteSpace(storableNode.RelativePath)))
			{
				if (string.IsNullOrWhiteSpace(item.AbsolutePath))
				{
					string path = Path.GetDirectoryName(item.RelativePath) ?? string.Empty;
					string path2 = Path.GetFileNameWithoutExtension(item.RelativePath) ?? string.Empty;
					string relativePath = Path.Combine(path, path2);
					Document.AddFile(FileProvider.BuildFile(item, nodeParser, BuildPath, relativePath, DocumentLogger));
				}
				else
				{
					Document.AddFile(new FileItem(item.AbsolutePath, item.RelativePath, item.Value.ToString()));
				}
			}
		}

		private void InitContentBuilder()
		{
			FileProvider.FileContentSaveParser.Initialize();
		}
	}
}
