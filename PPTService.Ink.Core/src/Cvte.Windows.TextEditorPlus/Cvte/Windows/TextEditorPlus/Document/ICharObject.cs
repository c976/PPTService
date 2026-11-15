using System;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public interface ICharObject : IEquatable<string>, ICloneable
	{
		string ToText();
	}
}
