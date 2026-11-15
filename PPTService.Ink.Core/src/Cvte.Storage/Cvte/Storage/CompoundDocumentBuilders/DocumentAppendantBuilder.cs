using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.CompoundDocumentBuilders
{
	
	
	internal class DocumentAppendantBuilder
	{
		private Func<SaveInfo, string, FileItem> BuildFileItemFunc { get; }

		public CompoundDocumentLogger Logger { get; set; }

		public CompoundDocument OriginCompoundDocument { get; set; }

		public DocumentAppendantBuilder(Func<SaveInfo, string, FileItem> buildFileItemFunc)
		{
			BuildFileItemFunc = buildFileItemFunc;
		}

		public IList<FileItem> Build(ICompoundDocumentAppendant appendant)
		{
			List<FileItem> list = new List<FileItem>();
			foreach (ICompoundDocumentAppendantItem appendantItem in appendant.GetAppendantItems())
			{
				FileItem fileItem = BuildOne(appendantItem);
				if (fileItem != null)
				{
					list.Add(fileItem);
				}
			}
			return list;
		}

		
		private FileItem BuildOne(ICompoundDocumentAppendantItem appendant)
		{
			if (appendant == null || string.IsNullOrWhiteSpace(appendant.RelativePath))
			{
				return null;
			}
			try
			{
				SaveInfo saveInfo = appendant.GetSaveInfo(OriginCompoundDocument);
				if (saveInfo == null)
				{
					return null;
				}
				return BuildFileItemFunc(saveInfo, appendant.RelativePath.Trim());
			}
			catch (Exception ex)
			{
				Logger?.Warning("Build AppendantItem Error; " + appendant.RelativePath + "; " + ex.Message);
				return null;
			}
		}

		internal static void AppendFileItem(ICompoundDocumentAppendant appendant, CompoundDocument document, CompoundDocumentLogger documentLogger, Func<SaveInfo, string, FileItem> buildFileItemFunc)
		{
			foreach (FileItem fileItem in new DocumentAppendantBuilder(buildFileItemFunc)
			{
				OriginCompoundDocument = document
			}.Build(appendant))
			{
				if (document.Files.Any( (FileItem f) => string.Equals(f.RelativePath, fileItem.RelativePath, StringComparison.OrdinalIgnoreCase)))
				{
					documentLogger.Warning("CompoundDocument Append Error, Exist Same RelativePath " + fileItem.RelativePath);
				}
				else
				{
					document.AddFile(fileItem);
				}
			}
		}
	}
}
