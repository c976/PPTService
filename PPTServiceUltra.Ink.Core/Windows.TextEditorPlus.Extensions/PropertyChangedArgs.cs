using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Extensions
{
	public class PropertyChangedArgs
	{
		public RunProperty OldValue { get; }

		public RunProperty NewValue { get; }

		public PropertyType Property { get; }

		public PropertyChangedArgs(PropertyType type, RunProperty oldValue, RunProperty newValue)
		{
			Property = type;
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}
