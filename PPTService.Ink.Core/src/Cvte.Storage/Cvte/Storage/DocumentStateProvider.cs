using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class DocumentStateProvider< TModel> where TModel : CompoundDocumentStorageModel
	{
		public DocumentStateManager<TModel> DocumentStateManager { get; }

		public CompoundDocument CurrentDocument { get; set; }

		public CompoundDocument BaseDocument => DocumentStateManager?.CurrentBaseDocument;

		public DocumentStateProvider(DocumentStateManager<TModel> manager)
		{
			DocumentStateManager = manager;
		}

		public void UpdateBaseDocument()
		{
			if (DocumentStateManager != null)
			{
				DocumentStateManager.CurrentBaseDocument = CurrentDocument;
			}
		}

		public TModel GetBaseModel()
		{
			DocumentStateManager<TModel> documentStateManager = DocumentStateManager;
			if (documentStateManager == null)
			{
				return null;
			}
			return documentStateManager.BaseModel;
		}
	}
}
