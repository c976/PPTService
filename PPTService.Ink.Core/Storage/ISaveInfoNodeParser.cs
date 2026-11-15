using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	internal interface ISaveInfoNodeParser : IMetadataProvider
	{
		StorableNode ParseValue(string value);

		string DeparseValue(StorableNode node);

		SaveInfo ParseNode(StorableNode currentNode);

		StorableNode DeparseNode(SaveInfo saveInfo);

		void SetReferenceManager(IReferenceManager referenceManager);

		void SetLogger(IStorageLogger logger);
	}
}
