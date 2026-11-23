using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	internal class TextCharVisual : ICharVisual
	{
		private Geometry _textGeometry;

		internal Geometry TextGeometry
		{
			get
			{
				return _textGeometry;
			}
			set
			{
				_textGeometry = value;
			}
		}

		internal TextCharGlyphData GlyphData { get; set; }

		internal Brush Foreground { get; set; }

		internal Matrix Matrix { get; set; }

		public Rect Bounds { get; set; }

		public CharFlowDirection Direction { get; set; } = CharFlowDirection.LeftToRight;

		public FlowDirection ActualDirection { get; set; }

		public bool IsIsLigature { get; set; }

		private void CorrectTextGeometry()
		{
			_textGeometry.SetMatrixTransform(Matrix);
		}

		public void UpdateMatrix()
		{
			_textGeometry.SetMatrixTransform(Matrix);
		}

		public override string ToString()
		{
			return GlyphData?.GlyphInfo?.UnicodeChar.ToString() ?? "<?>";
		}
	}
}
