using System;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public interface ICharObject : IEquatable<string>, ICloneable
	{
		string ToText();
	}
}
