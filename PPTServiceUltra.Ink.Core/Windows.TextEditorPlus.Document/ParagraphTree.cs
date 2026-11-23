using System;
using System.Collections;
using System.Collections.Generic;
using PPTServiceUltra.Ink.Core.Contracts;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	internal sealed class ParagraphTree : IList<ParagraphNode>, ICollection<ParagraphNode>, IEnumerable<ParagraphNode>, IEnumerable
	{
		private ParagraphNode _root;

		private const bool RED = true;

		private const bool BLACK = false;

		internal int ParagraphCount => _root.NodeTotalCount;

		ParagraphNode IList<ParagraphNode>.this[int index]
		{
			get
			{
				return GetParagraphNodeByNumber(1 + index);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		int ICollection<ParagraphNode>.Count => ParagraphCount;

		bool ICollection<ParagraphNode>.IsReadOnly => true;

		internal ParagraphTree()
		{
			_root = new ParagraphNode().InitNode();
		}

		internal ParagraphNode GetParagraphNodeByNumber(int paragraphNumber)
		{
			return GetNodeByIndex(paragraphNumber - 1);
		}

		internal ParagraphNode GetParagraphNodeByDocumentOffset(DocumentOffset documentOffset)
		{
			int num = documentOffset.Offset;
			if (num == _root.NodeTotalLength)
			{
				return _root.RightMost;
			}
			ParagraphNode paragraphNode = _root;
			while (true)
			{
				if (paragraphNode == null)
				{
					throw new InstanceNotReadyException("文档树还没构建完成，不能调用此方法，请确定文本已经渲染完成");
				}
				if (paragraphNode.Left != null && num < paragraphNode.Left.NodeTotalLength)
				{
					paragraphNode = paragraphNode.Left;
					continue;
				}
				if (paragraphNode.Left != null)
				{
					num -= paragraphNode.Left.NodeTotalLength;
				}
				num -= paragraphNode.TotalLength;
				if (num < 0)
				{
					break;
				}
				paragraphNode = paragraphNode.Right;
			}
			return paragraphNode;
		}

		internal ParagraphNode InsertParagraphNodeAfter(ParagraphNode node, int totalLength)
		{
			ParagraphNode paragraphNode = new ParagraphNode
			{
				TotalLength = totalLength
			};
			ParagraphNode newNode = paragraphNode.InitNode();
			if (node.Right == null)
			{
				InsertAsRight(node, newNode);
				return paragraphNode;
			}
			InsertAsLeft(node.Right.LeftMost, newNode);
			return paragraphNode;
		}

		internal void RemoveParagraphNode(ParagraphNode paragraphNode)
		{
			RemoveNode(paragraphNode);
			paragraphNode.IsDeleted = true;
		}

		internal void RebuildTree(List<ParagraphNode> documentParagraphNodes)
		{
			if (documentParagraphNodes.Count == 0)
			{
				throw new ArgumentException("documentParagraphNodes");
			}
			foreach (ParagraphNode documentParagraphNode in documentParagraphNodes)
			{
				documentParagraphNode.InitNode();
			}
			int treeHeight = GetTreeHeight(documentParagraphNodes.Count);
			_root = BuildTree(documentParagraphNodes, 0, documentParagraphNodes.Count, treeHeight);
			_root.Color = false;
		}

		internal static int GetTreeHeight(int size)
		{
			if (size == 0)
			{
				return 0;
			}
			return GetTreeHeight(size / 2) + 1;
		}

		internal static void UpdateAfterChildrenChange(ParagraphNode node)
		{
			if (node != null)
			{
				int num = 1;
				int num2 = node.TotalLength;
				if (node.Left != null)
				{
					num += node.Left.NodeTotalCount;
					num2 += node.Left.NodeTotalLength;
				}
				if (node.Right != null)
				{
					num += node.Right.NodeTotalCount;
					num2 += node.Right.NodeTotalLength;
				}
				if (num != node.NodeTotalCount || num2 != node.NodeTotalLength)
				{
					node.NodeTotalCount = num;
					node.NodeTotalLength = num2;
					UpdateAfterChildrenChange(node.Parent);
				}
			}
		}

		private static ParagraphNode BuildTree(IList<ParagraphNode> nodes, int start, int end, int subtreeHeight)
		{
			if (start == end)
			{
				return null;
			}
			int num = (start + end) / 2;
			ParagraphNode paragraphNode = nodes[num];
			paragraphNode.Left = BuildTree(nodes, start, num, subtreeHeight - 1);
			paragraphNode.Right = BuildTree(nodes, num + 1, end, subtreeHeight - 1);
			if (paragraphNode.Left != null)
			{
				paragraphNode.Left.Parent = paragraphNode;
			}
			if (paragraphNode.Right != null)
			{
				paragraphNode.Right.Parent = paragraphNode;
			}
			if (subtreeHeight == 1)
			{
				paragraphNode.Color = true;
			}
			UpdateAfterChildrenChange(paragraphNode);
			return paragraphNode;
		}

		private ParagraphNode GetNodeByIndex(int index)
		{
			ParagraphNode paragraphNode = _root;
			while (true)
			{
				if (paragraphNode.Left != null && index < paragraphNode.Left.NodeTotalCount)
				{
					paragraphNode = paragraphNode.Left;
					continue;
				}
				if (paragraphNode.Left != null)
				{
					index -= paragraphNode.Left.NodeTotalCount;
				}
				if (index == 0)
				{
					break;
				}
				index--;
				paragraphNode = paragraphNode.Right;
			}
			return paragraphNode;
		}

		private void InsertAsLeft(ParagraphNode parentNode, ParagraphNode newNode)
		{
			parentNode.Left = newNode;
			newNode.Parent = parentNode;
			newNode.Color = true;
			UpdateAfterChildrenChange(parentNode);
			FixTreeOnInsert(newNode);
		}

		private void InsertAsRight(ParagraphNode parentNode, ParagraphNode newNode)
		{
			parentNode.Right = newNode;
			newNode.Parent = parentNode;
			newNode.Color = true;
			UpdateAfterChildrenChange(parentNode);
			FixTreeOnInsert(newNode);
		}

		private void FixTreeOnInsert(ParagraphNode node)
		{
			ParagraphNode parent = node.Parent;
			if (parent == null)
			{
				node.Color = false;
			}
			else
			{
				if (!parent.Color)
				{
					return;
				}
				ParagraphNode parent2 = parent.Parent;
				ParagraphNode paragraphNode = Sibling(parent);
				if (paragraphNode != null && paragraphNode.Color)
				{
					parent.Color = false;
					paragraphNode.Color = false;
					parent2.Color = true;
					FixTreeOnInsert(parent2);
					return;
				}
				if (node == parent.Right && parent == parent2.Left)
				{
					RotateLeft(parent);
					node = node.Left;
				}
				else if (node == parent.Left && parent == parent2.Right)
				{
					RotateRight(parent);
					node = node.Right;
				}
				parent = node.Parent;
				parent2 = parent.Parent;
				parent.Color = false;
				parent2.Color = true;
				if (node == parent.Left && parent == parent2.Left)
				{
					RotateRight(parent2);
				}
				else
				{
					RotateLeft(parent2);
				}
			}
		}

		private void RemoveNode(ParagraphNode removedNode)
		{
			if (removedNode.Left != null && removedNode.Right != null)
			{
				ParagraphNode leftMost = removedNode.Right.LeftMost;
				RemoveNode(leftMost);
				ReplaceNode(removedNode, leftMost);
				leftMost.Left = removedNode.Left;
				if (leftMost.Left != null)
				{
					leftMost.Left.Parent = leftMost;
				}
				leftMost.Right = removedNode.Right;
				if (leftMost.Right != null)
				{
					leftMost.Right.Parent = leftMost;
				}
				leftMost.Color = removedNode.Color;
				UpdateAfterChildrenChange(leftMost);
				UpdateAfterChildrenChange(leftMost.Parent);
				return;
			}
			ParagraphNode parent = removedNode.Parent;
			ParagraphNode paragraphNode = removedNode.Left ?? removedNode.Right;
			ReplaceNode(removedNode, paragraphNode);
			if (parent != null)
			{
				UpdateAfterChildrenChange(parent);
			}
			if (!removedNode.Color)
			{
				if (paragraphNode != null && paragraphNode.Color)
				{
					paragraphNode.Color = false;
				}
				else
				{
					FixTreeOnDelete(paragraphNode, parent);
				}
			}
		}

		private void FixTreeOnDelete(ParagraphNode node, ParagraphNode parentNode)
		{
			if (parentNode == null)
			{
				return;
			}
			ParagraphNode paragraphNode = Sibling(node, parentNode);
			if (paragraphNode.Color)
			{
				parentNode.Color = true;
				paragraphNode.Color = false;
				if (node == parentNode.Left)
				{
					RotateLeft(parentNode);
				}
				else
				{
					RotateRight(parentNode);
				}
				paragraphNode = Sibling(node, parentNode);
			}
			if (!parentNode.Color && !paragraphNode.Color && !GetColor(paragraphNode.Left) && !GetColor(paragraphNode.Right))
			{
				paragraphNode.Color = true;
				FixTreeOnDelete(parentNode, parentNode.Parent);
				return;
			}
			if (parentNode.Color && !paragraphNode.Color && !GetColor(paragraphNode.Left) && !GetColor(paragraphNode.Right))
			{
				paragraphNode.Color = true;
				parentNode.Color = false;
				return;
			}
			if (node == parentNode.Left && !paragraphNode.Color && GetColor(paragraphNode.Left) && !GetColor(paragraphNode.Right))
			{
				paragraphNode.Color = true;
				paragraphNode.Left.Color = false;
				RotateRight(paragraphNode);
			}
			else if (node == parentNode.Right && !paragraphNode.Color && GetColor(paragraphNode.Right) && !GetColor(paragraphNode.Left))
			{
				paragraphNode.Color = true;
				paragraphNode.Right.Color = false;
				RotateLeft(paragraphNode);
			}
			paragraphNode = Sibling(node, parentNode);
			paragraphNode.Color = parentNode.Color;
			parentNode.Color = false;
			if (node == parentNode.Left)
			{
				if (paragraphNode.Right != null)
				{
					paragraphNode.Right.Color = false;
				}
				RotateLeft(parentNode);
			}
			else
			{
				if (paragraphNode.Left != null)
				{
					paragraphNode.Left.Color = false;
				}
				RotateRight(parentNode);
			}
		}

		private void ReplaceNode(ParagraphNode replacedNode, ParagraphNode newNode)
		{
			if (replacedNode.Parent == null)
			{
				_root = newNode;
			}
			else if (replacedNode.Parent.Left == replacedNode)
			{
				replacedNode.Parent.Left = newNode;
			}
			else
			{
				replacedNode.Parent.Right = newNode;
			}
			if (newNode != null)
			{
				newNode.Parent = replacedNode.Parent;
			}
			replacedNode.Parent = null;
		}

		private void RotateLeft(ParagraphNode p)
		{
			ParagraphNode right = p.Right;
			ReplaceNode(p, right);
			p.Right = right.Left;
			if (p.Right != null)
			{
				p.Right.Parent = p;
			}
			right.Left = p;
			p.Parent = right;
			UpdateAfterChildrenChange(p);
		}

		private void RotateRight(ParagraphNode p)
		{
			ParagraphNode left = p.Left;
			ReplaceNode(p, left);
			p.Left = left.Right;
			if (p.Left != null)
			{
				p.Left.Parent = p;
			}
			left.Right = p;
			p.Parent = left;
			UpdateAfterChildrenChange(p);
		}

		private static ParagraphNode Sibling(ParagraphNode node)
		{
			if (node != node.Parent.Left)
			{
				return node.Parent.Left;
			}
			return node.Parent.Right;
		}

		private static ParagraphNode Sibling(ParagraphNode node, ParagraphNode parentNode)
		{
			if (node != parentNode.Left)
			{
				return parentNode.Left;
			}
			return parentNode.Right;
		}

		private static bool GetColor(ParagraphNode node)
		{
			return node?.Color ?? false;
		}

		int IList<ParagraphNode>.IndexOf(ParagraphNode item)
		{
			if (item == null || item.IsDeleted)
			{
				return -1;
			}
			int num = item.ParagraphNumber - 1;
			if (num < ParagraphCount && GetNodeByIndex(num) == item)
			{
				return num;
			}
			return -1;
		}

		void IList<ParagraphNode>.Insert(int index, ParagraphNode item)
		{
			throw new NotSupportedException();
		}

		void IList<ParagraphNode>.RemoveAt(int index)
		{
			throw new NotSupportedException();
		}

		void ICollection<ParagraphNode>.Add(ParagraphNode item)
		{
			throw new NotSupportedException();
		}

		void ICollection<ParagraphNode>.Clear()
		{
			throw new NotSupportedException();
		}

		bool ICollection<ParagraphNode>.Contains(ParagraphNode item)
		{
			return ((IList<ParagraphNode>)this).IndexOf(item) >= 0;
		}

		void ICollection<ParagraphNode>.CopyTo(ParagraphNode[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Length < ParagraphCount)
			{
				throw new ArgumentException("The array is too small", "array");
			}
			if (arrayIndex < 0 || arrayIndex + ParagraphCount > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, "Value must be between 0 and " + (array.Length - ParagraphCount));
			}
			using (IEnumerator<ParagraphNode> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ParagraphNode current = enumerator.Current;
					array[arrayIndex++] = current;
				}
			}
		}

		bool ICollection<ParagraphNode>.Remove(ParagraphNode item)
		{
			throw new NotSupportedException();
		}

		public IEnumerator<ParagraphNode> GetEnumerator()
		{
			return Enumerate();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private IEnumerator<ParagraphNode> Enumerate()
		{
			for (ParagraphNode paragraphNode = _root.LeftMost; paragraphNode != null; paragraphNode = paragraphNode.NextNode)
			{
				yield return paragraphNode;
			}
		}
	}
}
