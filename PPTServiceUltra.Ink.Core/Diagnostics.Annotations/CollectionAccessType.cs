using System;

namespace PPTServiceUltra.Ink.Core.Diagnostics.Annotations
{
	[Flags]
	public enum CollectionAccessType
	{
		None = 0,
		Read = 1,
		ModifyExistingContent = 2,
		UpdatedContent = 6
	}
}
