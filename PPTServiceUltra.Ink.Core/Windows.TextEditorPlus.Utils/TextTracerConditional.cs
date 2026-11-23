namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class TextTracerConditional
	{
		private readonly bool _isTrue;

		public TextTracerConditional(bool isTrue)
		{
			_isTrue = isTrue;
		}

		public bool Check()
		{
			return _isTrue;
		}

		public static bool operator true(TextTracerConditional textTracerConditional)
		{
			return textTracerConditional._isTrue;
		}

		public static bool operator false(TextTracerConditional textTracerConditional)
		{
			return textTracerConditional._isTrue;
		}
	}
}
