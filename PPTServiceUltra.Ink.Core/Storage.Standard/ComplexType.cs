using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.Standard
{
	
	
	public abstract class ComplexType : IStorableType
	{
		protected IReferenceManager ReferenceManager;

		public abstract StorableNode Deparse();

		public abstract void Parse(StorableNode node);

		public void SetReferenceManager(IReferenceManager referenceManager)
		{
			ReferenceManager = referenceManager;
		}
	}
}
