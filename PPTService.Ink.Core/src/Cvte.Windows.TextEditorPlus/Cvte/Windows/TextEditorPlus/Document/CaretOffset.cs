using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public struct CaretOffset
	{
		private int _offset;

		private bool _isAtParagraphStart;

		public static readonly CaretOffset Invalid = new CaretOffset
		{
			Offset = -2
		};

		public static CaretOffset Default { get; } = new CaretOffset
		{
			Offset = -1,
			IsAtParagraphStart = true,
			IsAtLineStart = true
		};

		public int Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;
				if (_offset == -1)
				{
					IsAtParagraphStart = true;
				}
			}
		}

		public bool IsAtLineStart
		{
			
			get;
			set;
		}

		public bool IsAtParagraphStart
		{
			get
			{
				return _isAtParagraphStart;
			}
			set
			{
				_isAtParagraphStart = value;
				if (_isAtParagraphStart)
				{
					IsAtLineStart = true;
				}
			}
		}

		public CaretOffset Clone()
		{
			return new CaretOffset
			{
				IsAtLineStart = IsAtLineStart,
				IsAtParagraphStart = IsAtParagraphStart,
				Offset = Offset
			};
		}

		public bool Equals(CaretOffset other)
		{
			if (Offset == other.Offset && IsAtLineStart == other.IsAtLineStart)
			{
				return IsAtParagraphStart == other.IsAtParagraphStart;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is CaretOffset)
			{
				CaretOffset other = (CaretOffset)obj;
				return Equals(other);
			}
			return false;
		}

		public override string ToString()
		{
			return $"IsAtParagraphStart:{IsAtParagraphStart};IsAtLineStart:{IsAtLineStart};Offset:{Offset}";
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public static bool operator !=(CaretOffset left, CaretOffset right)
		{
			if (left.Offset == right.Offset)
			{
				return left.IsAtLineStart != right.IsAtLineStart;
			}
			return true;
		}

		public static bool operator ==(CaretOffset left, CaretOffset right)
		{
			if (left.Offset == right.Offset)
			{
				return left.IsAtLineStart == right.IsAtLineStart;
			}
			return false;
		}
	}
}
