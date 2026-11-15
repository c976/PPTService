using System;
using System.Diagnostics;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class SimpleSegment : ISegment, IEquatable<SimpleSegment>
	{
		public static readonly SimpleSegment Invalid = new SimpleSegment(0, 0);

		public const int UpperLimit = 1000000000;

		public int StartOffset { get; }

		public int Length { get; }

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

		[DebuggerStepThrough]
		public SimpleSegment(int offset, int length)
		{
			if (offset < -1)
			{
				throw new InvalidOperationException("offset cannot less than -1");
			}
			if (length < 0)
			{
				throw new InvalidOperationException("length cannot less than 0");
			}
			if (offset > 1000000000)
			{
				throw new InvalidOperationException($"offset cannot larger than {1000000000}");
			}
			if (length > 1000000000)
			{
				throw new InvalidOperationException($"length cannot larger than {1000000000}");
			}
			StartOffset = offset;
			Length = length;
		}

		private static bool Equals(SimpleSegment left, SimpleSegment right)
		{
			if ((object)left == null && (object)right == null)
			{
				return true;
			}
			if ((object)left == null || (object)right == null)
			{
				return false;
			}
			return left.Equals(right);
		}

		public bool Equals(SimpleSegment other)
		{
			if ((object)other == null)
			{
				return false;
			}
			if (StartOffset == other.StartOffset)
			{
				return Length == other.Length;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((SimpleSegment)obj);
		}

		public override int GetHashCode()
		{
			return (StartOffset * 397) ^ Length;
		}

		public static bool operator ==(SimpleSegment left, SimpleSegment right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SimpleSegment left, SimpleSegment right)
		{
			return !Equals(left, right);
		}
	}
}
