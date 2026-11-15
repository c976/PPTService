using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Diagnostics.Annotations;
using PPTService.Ink.Core.Windows.TextEditorPlus.Document.AttachedProperty;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class TextEditorDecorationCollection : List<TextEditorDecoration>, IEquatable<TextEditorDecorationCollection>, IAttachedRunProperty
	{
		public TextEditorDecorationCollection()
		{
		}

		private TextEditorDecorationCollection([NotNull] IEnumerable<TextEditorDecoration> collection)
			: base(collection)
		{
		}

		public bool Equals(TextEditorDecorationCollection other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Count != other.Count)
			{
				return false;
			}
			for (int i = 0; i < Count; i++)
			{
				if (!base[i].Equals(other[i]))
				{
					return false;
				}
			}
			return true;
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
			TextEditorDecorationCollection textEditorDecorationCollection = obj as TextEditorDecorationCollection;
			if (textEditorDecorationCollection != null)
			{
				return Equals(textEditorDecorationCollection);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public IAttachedRunProperty Clone()
		{
			return new TextEditorDecorationCollection(this);
		}
	}
}
