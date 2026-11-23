using System;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public sealed class ParagraphNode : IParagraphSegment, ISegment
	{
		internal bool Color { get; set; }

		internal ParagraphNode Left { get; set; }

		internal ParagraphNode Right { get; set; }

		internal ParagraphNode Parent { get; set; }

		internal ParagraphNode LeftMost
		{
			get
			{
				ParagraphNode paragraphNode = this;
				while (paragraphNode.Left != null)
				{
					paragraphNode = paragraphNode.Left;
				}
				return paragraphNode;
			}
		}

		internal ParagraphNode RightMost
		{
			get
			{
				ParagraphNode paragraphNode = this;
				while (paragraphNode.Right != null)
				{
					paragraphNode = paragraphNode.Right;
				}
				return paragraphNode;
			}
		}

		internal ParagraphNode NextNode
		{
			get
			{
				if (Right != null)
				{
					return Right.LeftMost;
				}
				ParagraphNode paragraphNode = this;
				ParagraphNode paragraphNode2;
				do
				{
					paragraphNode2 = paragraphNode;
					paragraphNode = paragraphNode.Parent;
				}
				while (paragraphNode != null && paragraphNode.Right == paragraphNode2);
				return paragraphNode;
			}
		}

		internal ParagraphNode PreviousNode
		{
			get
			{
				if (Left != null)
				{
					return Left.RightMost;
				}
				ParagraphNode paragraphNode = this;
				ParagraphNode paragraphNode2;
				do
				{
					paragraphNode2 = paragraphNode;
					paragraphNode = paragraphNode.Parent;
				}
				while (paragraphNode != null && paragraphNode.Left == paragraphNode2);
				return paragraphNode;
			}
		}

		public bool IsDeleted { get; set; }

		public int ParagraphNumber
		{
			get
			{
				if (IsDeleted)
				{
					throw new InvalidOperationException();
				}
				ParagraphNode paragraphNode = this;
				int num = 0;
				if (paragraphNode.Left != null)
				{
					num = paragraphNode.Left.NodeTotalCount;
				}
				while (paragraphNode.Parent != null)
				{
					if (paragraphNode == paragraphNode.Parent.Right)
					{
						if (paragraphNode.Parent.Left != null)
						{
							num += paragraphNode.Parent.Left.NodeTotalCount;
						}
						num++;
					}
					paragraphNode = paragraphNode.Parent;
				}
				return num + 1;
			}
		}

		public int StartOffset
		{
			get
			{
				if (IsDeleted)
				{
					throw new InvalidOperationException();
				}
				ParagraphNode paragraphNode = this;
				int num = 0;
				if (paragraphNode.Left != null)
				{
					num = paragraphNode.Left.NodeTotalLength;
				}
				while (paragraphNode.Parent != null)
				{
					if (paragraphNode == paragraphNode.Parent.Right)
					{
						if (paragraphNode.Parent.Left != null)
						{
							num += paragraphNode.Parent.Left.NodeTotalLength;
						}
						num += paragraphNode.Parent.TotalLength;
					}
					paragraphNode = paragraphNode.Parent;
				}
				return num;
			}
		}

		public int EndOffset => StartOffset + Length;

		public int FrontOffset
		{
			get
			{
				if (StartOffset <= EndOffset)
				{
					return StartOffset;
				}
				return EndOffset;
			}
		}

		public int BehindOffset
		{
			get
			{
				if (StartOffset <= EndOffset)
				{
					return EndOffset;
				}
				return StartOffset;
			}
		}

		public int Length => TotalLength - DelimiterLength;

		public int TotalLength { get; set; }

		public int DelimiterLength { get; set; }

		public int NodeTotalCount { get; set; }

		public int NodeTotalLength { get; set; }

		internal void ResetNode()
		{
			int totalLength = (DelimiterLength = 0);
			TotalLength = totalLength;
			bool isDeleted = (Color = false);
			IsDeleted = isDeleted;
			ParagraphNode paragraphNode = (Parent = null);
			ParagraphNode left = (Right = paragraphNode);
			Left = left;
		}

		internal ParagraphNode InitNode()
		{
			NodeTotalCount = 1;
			NodeTotalLength = TotalLength;
			return this;
		}
	}
}
