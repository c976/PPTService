using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	public class DocumentStateManager< TModel> where TModel : CompoundDocumentStorageModel
	{
		internal CompoundDocument CurrentBaseDocument { get; set; }

		internal TModel BaseModel { get; set; }

		public void UpdateBaseModel(TModel model)
		{
			BaseModel = model;
		}
	}
}
