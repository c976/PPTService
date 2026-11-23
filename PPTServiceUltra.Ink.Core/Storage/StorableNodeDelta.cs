using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal class StorableNodeDelta
	{
		public DeltaAction DeltaAction { get; }

		public StorableNode StorableNode { get; set; }

		public bool IsOrderRequired { get; set; }

		public IList<StorableNodeDelta> Children { get; private set; } = new List<StorableNodeDelta>();

		public StorableNodeDelta(DeltaAction deltaAction)
		{
			DeltaAction = deltaAction;
		}

		public void AddChild(StorableNodeDelta node)
		{
			if (Children == null)
			{
				Children = new List<StorableNodeDelta>();
			}
			Children.Add(node);
		}

		public void AddChild(IList<StorableNodeDelta> children)
		{
			if (children == null || children.Count == 0)
			{
				return;
			}
			foreach (StorableNodeDelta child in children)
			{
				AddChild(child);
			}
		}
	}
}
