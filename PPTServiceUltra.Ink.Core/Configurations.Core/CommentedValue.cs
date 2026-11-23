using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Configurations.Core
{
	
	
	
	[DebuggerDisplay("{Value,nq} // {Comment,nq}")]
	public struct CommentedValue< T> : IEquatable<CommentedValue<T>>
	{
		public T Value { get; }

		public string Comment { get; }

		public CommentedValue(T value, string comment = "")
		{
			Value = value;
			if (comment == null)
			{
				throw new ArgumentNullException("comment");
			}
			Comment = comment;
		}

		
		public override bool Equals(object obj)
		{
			if (obj is CommentedValue<T>)
			{
				CommentedValue<T> other = (CommentedValue<T>)obj;
				return Equals(other);
			}
			return false;
		}

		public bool Equals( CommentedValue<T> other)
		{
			if (EqualityComparer<T>.Default.Equals(Value, other.Value))
			{
				return string.Equals(Comment, other.Comment, StringComparison.Ordinal);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (unchecked(618185720 * -1521134295) + EqualityComparer<T>.Default.GetHashCode(Value)) * -1521134295 + StringComparer.Ordinal.GetHashCode(Comment);
		}

		public static bool operator ==( CommentedValue<T> left,  CommentedValue<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=( CommentedValue<T> left,  CommentedValue<T> right)
		{
			return !(left == right);
		}
	}
}
