using System.Collections.Generic;

namespace PPTService.Ink.Core.Windows.TextEditorPlus.Document
{
	public class RunPropertyInfo
	{
		public List<int> PreIndexes;

		public List<int> AfterIndexes;

		public RunProperty PreCurrentRunProperty { get; set; }

		public RunProperty AfterCurrentRunProperty { get; set; }
	}
}
