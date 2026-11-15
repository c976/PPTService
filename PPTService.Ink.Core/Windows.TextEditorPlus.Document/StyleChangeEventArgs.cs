using System;
using System.Diagnostics;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class StyleChangeEventArgs : EventArgs
	{
		public bool IsFromUndoRedo { get; set; }

		public ISegment ChangedSegment { get; set; }

		public PropertyType ChangedProperty { get; set; }

		[DebuggerStepThrough]
		public StyleChangeEventArgs(ISegment segment, PropertyType changedProperty)
		{
			ChangedSegment = segment;
			ChangedProperty = changedProperty;
		}
	}
}
