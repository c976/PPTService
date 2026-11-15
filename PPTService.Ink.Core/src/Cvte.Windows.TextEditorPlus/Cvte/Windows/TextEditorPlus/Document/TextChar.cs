using System;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class TextChar : ICharObject, IEquatable<string>, ICloneable
	{
		internal string TextCharObject { get; }

		public TextChar([NotNull] string textCharObject)
		{
			TextCharObject = textCharObject;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			TextChar textChar = obj as TextChar;
			if (textChar != null)
			{
				return Equals(textChar.TextCharObject);
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (TextCharObject == null)
			{
				return 0;
			}
			return TextCharObject.GetHashCode();
		}

		public object Clone()
		{
			return this;
		}

		public bool Equals(string str)
		{
			return TextCharObject.Equals(str);
		}

		public string ToText()
		{
			return TextCharObject;
		}
	}
}
