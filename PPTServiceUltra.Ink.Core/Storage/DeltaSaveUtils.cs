using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal static class DeltaSaveUtils
	{
		private static bool TryBuildDelta< TModel>(TModel model, TModel originModel, ReferenceProvider referenceProvider, ParserProvider parserProvider, out StorableNodeDelta delta, out List<MetadataSaveInfo> metadata) where TModel : CompoundDocumentStorageModel
		{
			IStorageLogger logger;
			ValueTuple<StorableNode, List<MetadataSaveInfo>> valueTuple = DocumentBuilder.BuildNode(originModel, parserProvider, referenceProvider, out logger);
			StorableNode item = valueTuple.Item1;
			List<MetadataSaveInfo> item2 = valueTuple.Item2;
			ValueTuple<StorableNode, List<MetadataSaveInfo>> valueTuple2 = DocumentBuilder.BuildNode(model, parserProvider, referenceProvider, out logger);
			StorableNode item3 = valueTuple2.Item1;
			List<MetadataSaveInfo> item4 = valueTuple2.Item2;
			delta = item3.BuildNodeDelta(item);
			item2.AddRange(item4);
			metadata = item2.Distinct(new MetadataComparer()).ToList();
			if (delta == null)
			{
				return false;
			}
			return true;
		}

		private static void FixInconsistentNodeName(StorableNode node)
		{
			if (!string.IsNullOrWhiteSpace(node.ConsistentName))
			{
				node.Name = node.ConsistentName;
			}
			foreach (StorableNode child in node.Children)
			{
				FixInconsistentNodeName(child);
			}
		}

		private static bool TryBuildDocumentNode< TModel>(FileProvider fileProvider, ReferenceProvider referenceProvider, ParserProvider parserProvider, CompoundDocument compoundDocument, out StorableNode documentNode, out CompoundDocumentLogger logger) where TModel : CompoundDocumentStorageModel
		{
			documentNode = DocumentReader.ReadNode<TModel>(compoundDocument, parserProvider, referenceProvider, fileProvider, out logger);
			if (documentNode == null)
			{
				return false;
			}
			FixInconsistentNodeName(documentNode);
			return true;
		}

		
		public static ValueTuple<StorableNode, List<MetadataSaveInfo>> BuildUpdatedNode< TModel>(TModel model, TModel originModel, CompoundDocument compoundDocument, ParserProvider parserProvider, ReferenceProvider referenceProvider, FileProvider fileProvider) where TModel : CompoundDocumentStorageModel
		{
			StorableNode documentNode;
			CompoundDocumentLogger logger;
			if (!TryBuildDocumentNode<TModel>(fileProvider, referenceProvider, parserProvider, compoundDocument, out documentNode, out logger))
			{
				return new ValueTuple<StorableNode, List<MetadataSaveInfo>>(null, new List<MetadataSaveInfo>());
			}
			StorableNode item = documentNode;
			originModel.Document = logger.Document;
			StorableNodeDelta delta;
			List<MetadataSaveInfo> metadata;
			if (TryBuildDelta(model, originModel, referenceProvider, parserProvider, out delta, out metadata))
			{
				item = UpdateNodeWithDelta(documentNode, delta);
			}
			return new ValueTuple<StorableNode, List<MetadataSaveInfo>>(item, metadata);
		}

		private static StorableNode UpdateNodeWithDelta(StorableNode node, StorableNodeDelta delta)
		{
			try
			{
				return StorableNodeDeltaUtils.UpdateNodeWithDelta(node, delta);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
