using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage.Standard
{
	
	
	public abstract class CompoundType : IStorableType
	{
		protected IReferenceManager ReferenceManager;

		internal string Name { get; set; }

		public abstract StorableNode Deparse();

		public abstract void Parse(StorableNode node);

		public void SetReferenceManager(IReferenceManager referenceManager)
		{
			ReferenceManager = referenceManager;
		}
	}
}
