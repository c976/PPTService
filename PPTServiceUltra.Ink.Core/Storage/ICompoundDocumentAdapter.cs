using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	public interface ICompoundDocumentAdapter< in TTarget>
	{
		bool IsLegal(TTarget target);

		bool TryGetVersion(TTarget target, out string version);

		bool TryLoad(TTarget target, out CompoundDocument document);

		void Save(CompoundDocument document, TTarget target);

		void Save(CompoundDocument originDocument, TTarget target, CompoundDocument newDocument);
	}
}
