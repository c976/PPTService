using System;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public class DocumentOffset : IEquatable<DocumentOffset>, IEquatable<int>
	{
		public static readonly DocumentOffset DefaultDocumentOffset = new DocumentOffset(-1);

		public int Offset { get; }

		public DocumentOffset(int offset)
		{
			if (offset < DefaultDocumentOffset?.Offset)
			{
				throw new ArgumentOutOfRangeException("文档的偏移量最小值不能小于 -1 默认值");
			}
			Offset = offset;
		}

		public static implicit operator int(DocumentOffset offset)
		{
			return offset.Offset;
		}

		public static implicit operator DocumentOffset(int offset)
		{
			return new DocumentOffset(offset);
		}

		public static bool operator >(int a, DocumentOffset b)
		{
			if ((object)b == null)
			{
				return false;
			}
			return a > b.Offset;
		}

		public static bool operator >(DocumentOffset a, DocumentOffset b)
		{
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.Offset > b.Offset;
		}

		public static bool operator >=(int a, DocumentOffset b)
		{
			if ((object)b == null)
			{
				return false;
			}
			return a >= b.Offset;
		}

		public static bool operator >=(DocumentOffset a, DocumentOffset b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.Offset >= b.Offset;
		}

		public static bool operator <(int a, DocumentOffset b)
		{
			if ((object)b == null)
			{
				return false;
			}
			return a < b.Offset;
		}

		public static bool operator <(DocumentOffset a, DocumentOffset b)
		{
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.Offset < b.Offset;
		}

		public static bool operator <=(int a, DocumentOffset b)
		{
			if ((object)b == null)
			{
				return false;
			}
			return a <= b.Offset;
		}

		public static bool operator <=(DocumentOffset a, DocumentOffset b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.Offset <= b.Offset;
		}

		public static bool operator >(DocumentOffset a, int b)
		{
			if ((object)a == null)
			{
				return false;
			}
			return a.Offset > b;
		}

		public static bool operator >=(DocumentOffset b, int a)
		{
			if ((object)b == null)
			{
				return false;
			}
			return b.Offset >= a;
		}

		public static bool operator <(DocumentOffset b, int a)
		{
			if ((object)b == null)
			{
				return false;
			}
			return b.Offset < a;
		}

		public static bool operator <=(DocumentOffset b, int a)
		{
			if ((object)b == null)
			{
				return false;
			}
			return b.Offset <= a;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			DocumentOffset documentOffset = obj as DocumentOffset;
			if ((object)documentOffset != null)
			{
				return Offset.Equals(documentOffset.Offset);
			}
			return false;
		}

		public bool Equals(DocumentOffset other)
		{
			if ((object)other != null)
			{
				return Offset.Equals(other.Offset);
			}
			return false;
		}

		public bool Equals(int other)
		{
			return Offset.Equals(other);
		}

		public static bool operator ==(DocumentOffset b, DocumentOffset a)
		{
			if ((object)b == a)
			{
				return true;
			}
			if ((object)b == null || (object)a == null)
			{
				return false;
			}
			return b.Equals(a);
		}

		public static bool operator ==(DocumentOffset b, int a)
		{
			return b?.Equals(a) ?? false;
		}

		public static bool operator !=(DocumentOffset b, int a)
		{
			return !(b == a);
		}

		public static bool operator ==(int a, DocumentOffset b)
		{
			return b?.Equals(a) ?? false;
		}

		public static bool operator !=(int a, DocumentOffset b)
		{
			return !(b == a);
		}

		public static bool operator !=(DocumentOffset b, DocumentOffset a)
		{
			return !(b == a);
		}

		public override int GetHashCode()
		{
			return Offset;
		}
	}
}
