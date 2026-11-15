using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Storage
{
	
	
	internal static class StorableNodeDeltaUtils
	{
		
		private abstract class NodeUpdater
		{
			
			public abstract void Update(StorableNode node, StorableNodeDelta delta);
		}

		
		private class AddUpdater : NodeUpdater
		{
			
			public override void Update(StorableNode node, StorableNodeDelta delta)
			{
				node.Children.Add(delta.StorableNode);
			}
		}

		
		private class DeleteUpdater : NodeUpdater
		{
			
			public override void Update(StorableNode node, StorableNodeDelta delta)
			{
				List<StorableNode> list = node.Children.Where((StorableNode storableNode) => storableNode.InternalKey == delta.StorableNode.InternalKey).ToList();
				if (list.Count == 0)
				{
					throw new InvalidOperationException("找不到对应的删除节点");
				}
				if (list.Count > 1)
				{
					throw new InvalidOperationException("存在重复的InternalKey");
				}
				node.Children.Remove(list[0]);
			}
		}

		
		private class UpdateUpdater : NodeUpdater
		{
			
			public override void Update(StorableNode node, StorableNodeDelta delta)
			{
				List<StorableNode> list = node.Children.Where((StorableNode storableNode2) => storableNode2.InternalKey == delta.StorableNode.InternalKey).ToList();
				if (list.Count == 0)
				{
					throw new InvalidOperationException("找不到对应的更新节点");
				}
				if (list.Count > 1)
				{
					throw new InvalidOperationException("存在重复的InternalKey");
				}
				StorableNode storableNode = list[0];
				storableNode.Value = delta.StorableNode.Value;
				storableNode.AbsolutePath = delta.StorableNode.AbsolutePath;
				storableNode.RelativePath = delta.StorableNode.RelativePath;
				if (!delta.IsOrderRequired && delta.Children.Count == 0)
				{
					storableNode.Children.Clear();
					storableNode.AddChild(delta.StorableNode.Children);
				}
				else
				{
					StorableNodeCore(storableNode, delta);
				}
			}
		}

		private static readonly Lazy<NodeUpdater> AddNodeUpdater = new Lazy<NodeUpdater>(() => new AddUpdater());

		private static readonly Lazy<NodeUpdater> DeleteNodeUpdater = new Lazy<NodeUpdater>(() => new DeleteUpdater());

		private static readonly Lazy<NodeUpdater> UpdateNodeUpdater = new Lazy<NodeUpdater>(() => new UpdateUpdater());

		internal static StorableNode UpdateNodeWithDelta(StorableNode node, StorableNodeDelta delta)
		{
			if (delta.DeltaAction != DeltaAction.Update)
			{
				throw new ArgumentException("delta 的根节点必须为update");
			}
			if (node.InternalKey != delta.StorableNode.InternalKey)
			{
				throw new ArgumentException("根节点的InternalKey必须为相同");
			}
			if (!delta.IsOrderRequired && delta.Children.Count == 0)
			{
				node.FillFrom(delta.StorableNode);
				return node;
			}
			return StorableNodeCore(node, delta);
		}

		private static StorableNode StorableNodeCore(StorableNode node, StorableNodeDelta delta)
		{
			List<string> list = node.Children.Select( (StorableNode x) => x.InternalKey).ToList();
			List<string> newOrder = delta.StorableNode.Children.Select( (StorableNode x) => x.InternalKey).ToList();
			foreach (StorableNodeDelta child in delta.Children)
			{
				GetNodeUpdater(child.DeltaAction).Update(node, child);
			}
			if ((node.IsArray || delta.StorableNode.IsArray) && !StorableNodeDeltaExtension.HasDuplicatedItem(node.Children.ToList()))
			{
				Dictionary<string, StorableNode> nodes = node.Children.ToDictionary( (StorableNode storableNode) => storableNode.InternalKey);
				List<string> list2 = list.Where( (string x) => nodes.ContainsKey(x) && !newOrder.Contains(x)).ToList();
				node.Children.Clear();
				List<StorableNode> list3 = new List<StorableNode>();
				foreach (StorableNode child2 in delta.StorableNode.Children)
				{
					list3.Add(nodes[child2.InternalKey]);
				}
				for (int num = 0; num < list2.Count; num++)
				{
					string text = list2[num];
					int num2 = list.IndexOf(text);
					if (num2 > list3.Count)
					{
						for (; num < list2.Count; num++)
						{
							text = list2[num];
							list3.Add(nodes[text]);
						}
					}
					else
					{
						list3.Insert(num2, nodes[text]);
					}
				}
				node.AddChild(list3);
			}
			return node;
		}

		private static NodeUpdater GetNodeUpdater(DeltaAction deltaAction)
		{
			switch (deltaAction)
			{
			case DeltaAction.Add:
				return AddNodeUpdater.Value;
			case DeltaAction.Delete:
				return DeleteNodeUpdater.Value;
			case DeltaAction.Update:
				return UpdateNodeUpdater.Value;
			default:
				throw new ArgumentException("没有实现该类型的Updater");
			}
		}
	}
}
