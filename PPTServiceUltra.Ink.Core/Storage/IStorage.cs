using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface IStorage< TModel,  in TTarget> where TModel : StorageModel
	{
		bool Saved { get; }

		TModel Open(TTarget target);

		void SaveAs(TModel model, TTarget target);

		void Save(TModel model);

		void New();
	}
}
