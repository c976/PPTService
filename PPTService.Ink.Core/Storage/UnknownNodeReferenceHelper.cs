using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Storage.Log;
using PPTService.Ink.Core.Storage.Standard;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal class UnknownNodeReferenceHelper
	{
		public static void RecordReference( IReferenceManager referenceManager, StorableNode node)
		{
			if (referenceManager == null)
			{
				return;
			}
			foreach (string item in GetReferenceId(node))
			{
				try
				{
					referenceManager.AddReference(item);
				}
				catch (ReferenceMissingException ex)
				{
					StorageStaticLoggerFactory.Logger?.Warning("未知节点中的引用未找到", ex);
				}
			}
		}

		private static IList<string> GetReferenceId(StorableNode node)
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrWhiteSpace(node.StringValue) && node.StringValue.StartsWith("id://"))
			{
				IdUri idUri = new IdUri(node.StringValue);
				list.Add(idUri.Value);
			}
			foreach (StorableNode child in node.Children)
			{
				IList<string> referenceId = GetReferenceId(child);
				list.AddRange(referenceId);
			}
			return list;
		}
	}
}
