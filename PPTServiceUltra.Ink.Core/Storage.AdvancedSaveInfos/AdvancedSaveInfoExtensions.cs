using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage.AdvancedSaveInfos
{
	
	
	public static class AdvancedSaveInfoExtensions
	{
		
		public static string GetMetadataNodeName(this StorableNode storableNode)
		{
			return (storableNode.Children.FirstOrDefault( (StorableNode c) => string.Equals(c.Name, "SaveInfoMetadata", StringComparison.OrdinalIgnoreCase))?.Children.FirstOrDefault( (StorableNode n) => string.Equals(n.Name, "NodeName", StringComparison.Ordinal)))?.Value.ToString();
		}

		public static bool GetIsSupportFallback(this StorableNode storableNode)
		{
			StorableNode storableNode2 = storableNode.Children.FirstOrDefault( (StorableNode c) => string.Equals(c.Name, "SaveInfoMetadata", StringComparison.OrdinalIgnoreCase))?.Children.FirstOrDefault( (StorableNode n) => string.Equals(n.Name, "SupportsFallback", StringComparison.Ordinal));
			if (storableNode2 == null)
			{
				return false;
			}
			return string.Equals(storableNode2.StringValue.Trim(), "true", StringComparison.OrdinalIgnoreCase);
		}
	}
}
