using System;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document
{
	public interface ICharFeature : ICharObject, IEquatable<string>, ICloneable
	{
		bool HitTestEnable { get; }
	}
}
