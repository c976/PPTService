using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;
using PPTServiceUltra.Ink.Core.Linq;

namespace PPTServiceUltra.Ink.Core.Storage
{
	
	
	internal static class StorableNodeDeltaExtension
	{
		public static bool EqualsTo(this StorableNode node, StorableNode otherNode)
		{
			if (otherNode == null)
			{
				return false;
			}
			if (node.InternalKey != otherNode.InternalKey)
			{
				return false;
			}
			if (node.Children.Count != otherNode.Children.Count)
			{
				return false;
			}
			if (!node.HasChild && node.StringValue != otherNode.StringValue)
			{
				return false;
			}
			if (node.RelativePath != otherNode.RelativePath)
			{
				return false;
			}
			if (!node.IsDeltaSupported || !otherNode.IsDeltaSupported)
			{
				return false;
			}
			List<StorableNode> list = node.Children.ToList();
			List<StorableNode> list2 = otherNode.Children.ToList();
			list.Sort( (StorableNode storableNode, StorableNode storableNode2) => string.Compare(storableNode.InternalKey, storableNode2.InternalKey));
			list2.Sort( (StorableNode storableNode, StorableNode storableNode2) => string.Compare(storableNode.InternalKey, storableNode2.InternalKey));
			for (int num = 0; num < list.Count; num++)
			{
				if (!list[num].EqualsTo(list2[num]))
				{
					return false;
				}
			}
			if ((node.IsArray || otherNode.IsArray) && node.Children.Count > 0)
			{
				IEnumerable<string> first = node.Children.Select( (StorableNode x) => x.InternalKey);
				IEnumerable<string> second = otherNode.Children.Select( (StorableNode x) => x.InternalKey);
				return first.SequenceEqual(second);
			}
			return true;
		}

		public static void FillFrom(this StorableNode node, StorableNode otherNode)
		{
			node.InternalKey = otherNode.InternalKey;
			node.AbsolutePath = otherNode.AbsolutePath;
			node.RelativePath = otherNode.RelativePath;
			node.Name = otherNode.Name;
			node.Value = otherNode.Value;
			node.DeltaSaveTerminalNode = otherNode.DeltaSaveTerminalNode;
			node.ConsistentName = otherNode.ConsistentName;
			node.IsArray = otherNode.IsArray;
			node.Children.Clear();
			otherNode.Children.ForEach( (StorableNode storableNode) =>
			{
				node.Children.Add(storableNode);
			});
		}

		[CanBeNull]
		public static StorableNodeDelta BuildNodeDelta(this StorableNode node, StorableNode otherNode)
		{
			if (!node.IsDeltaSupported || !otherNode.IsDeltaSupported)
			{
				return new StorableNodeDelta(DeltaAction.Update)
				{
					StorableNode = node
				};
			}
			if (node.EqualsTo(otherNode))
			{
				return null;
			}
			StorableNodeDelta storableNodeDelta = new StorableNodeDelta(DeltaAction.Update)
			{
				StorableNode = node
			};
			if (node.DeltaSaveTerminalNode || otherNode.DeltaSaveTerminalNode)
			{
				return storableNodeDelta;
			}
			if (node.IsArray)
			{
				storableNodeDelta.IsOrderRequired = true;
			}
			List<StorableNode> nodeChildren = node.Children.ToList();
			List<StorableNode> otherNodeChildren = otherNode.Children.ToList();
			if (HasDuplicatedItem(nodeChildren) || HasDuplicatedItem(otherNodeChildren))
			{
				return storableNodeDelta;
			}
			HashSet<string> nodeSet = new HashSet<string>(nodeChildren.Select( (StorableNode storableNode2) => storableNode2.InternalKey));
			List<StorableNode> list = otherNodeChildren.Where( (StorableNode n) => !nodeSet.Contains(n.InternalKey)).ToList();
			list.ForEach( (StorableNode n) =>
			{
				otherNodeChildren.Remove(n);
			});
			storableNodeDelta.AddChild(list.Select( (StorableNode n) => new StorableNodeDelta(DeltaAction.Delete)
			{
				StorableNode = n
			}).ToList());
			HashSet<string> otherNodeSet = new HashSet<string>(otherNodeChildren.Select( (StorableNode storableNode2) => storableNode2.InternalKey));
			List<StorableNode> list2 = nodeChildren.Where( (StorableNode n) => !otherNodeSet.Contains(n.InternalKey)).ToList();
			list2.ForEach( (StorableNode n) =>
			{
				nodeChildren.Remove(n);
			});
			storableNodeDelta.AddChild(list2.Select( (StorableNode n) => new StorableNodeDelta(DeltaAction.Add)
			{
				StorableNode = n
			}).ToList());
			foreach (StorableNode newNode in nodeChildren)
			{
				StorableNode storableNode = otherNodeChildren.Find( (StorableNode n) => n.InternalKey == newNode.InternalKey);
				otherNodeChildren.Remove(storableNode);
				StorableNodeDelta storableNodeDelta2 = newNode.BuildNodeDelta(storableNode);
				if (storableNodeDelta2 != null)
				{
					storableNodeDelta.AddChild(storableNodeDelta2);
				}
			}
			return storableNodeDelta;
		}

		internal static bool HasDuplicatedItem(List<StorableNode> nodeChildren)
		{
			return nodeChildren.Any( (StorableNode n) => nodeChildren.Where( (StorableNode node) => node != n).Any( (StorableNode s) => s.InternalKey == n.InternalKey));
		}
	}
}
