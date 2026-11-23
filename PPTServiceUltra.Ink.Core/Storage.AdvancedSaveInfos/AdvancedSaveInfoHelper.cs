using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.AdvancedSaveInfos
{
	
	
	internal static class AdvancedSaveInfoHelper
	{
		public static void AppendUnknownNode(this StorableNode currentNode, SaveInfo saveInfo, ISaveInfoNodeParser parser, IReferenceManager referenceManager)
		{
			if (saveInfo.UnknownProperties == null || saveInfo.UnknownProperties.Count == 0)
			{
				return;
			}
			foreach (StorableNode unknownNode in saveInfo.UnknownProperties)
			{
				if (currentNode.IsSupportFallback(parser))
				{
					UnknownNodeReferenceHelper.RecordReference(referenceManager, unknownNode);
					if (currentNode.Children.All( (StorableNode c) => !string.Equals(c.Name, unknownNode.Name, StringComparison.OrdinalIgnoreCase)))
					{
						currentNode.AddChild(unknownNode);
					}
				}
			}
		}

		public static bool IsSupportFallback(this StorableNode storableNode, ISaveInfoNodeParser parser)
		{
			return storableNode.IsSupportFallback(parser.ParseNode);
		}

		public static bool IsSupportFallback(this StorableNode storableNode, Func<StorableNode, SaveInfo> parser)
		{
			return storableNode.GetAdvancedSaveInfoMetadata(parser)?.SupportsFallback ?? false;
		}

		
		public static AdvancedSaveInfoMetadata GetAdvancedSaveInfoMetadata(this StorableNode storableNode, ISaveInfoNodeParser parser)
		{
			return storableNode.GetAdvancedSaveInfoMetadata(parser.ParseNode);
		}

		
		public static AdvancedSaveInfoMetadata GetAdvancedSaveInfoMetadata(this StorableNode storableNode, Func<StorableNode, SaveInfo> parser)
		{
			StorableNode storableNode2 = storableNode.Children.FirstOrDefault( (StorableNode c) => string.Equals(c.Name, "SaveInfoMetadata", StringComparison.OrdinalIgnoreCase));
			if (storableNode2 == null)
			{
				return null;
			}
			return parser(storableNode2) as AdvancedSaveInfoMetadata;
		}
	}
}
