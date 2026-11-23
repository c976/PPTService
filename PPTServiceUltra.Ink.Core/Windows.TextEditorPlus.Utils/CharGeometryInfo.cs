using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Document;
using PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Utils
{
	internal class CharGeometryInfo
	{
		internal Geometry Geometry { get; set; }

		internal Rect HorizontalBounds { get; }

		internal Size Size { get; set; }

		internal TextCharGlyphData Data { get; set; }

		internal bool IsDirty { get; set; }

		internal Matrix OriginalMatrix { get; private set; }

		internal CharGeometryInfo(Geometry geometry, Size size, TextCharGlyphData data)
		{
			Geometry = geometry;
			Size = size;
			HorizontalBounds = Geometry.Bounds;
			Data = data;
		}

		internal void SetOriginalMatrix(Matrix matrix)
		{
			OriginalMatrix = matrix;
		}

		internal void RecoverOriginalMatrix()
		{
			Geometry.SetMatrixTransform(OriginalMatrix);
		}

		[Conditional("DEBUG")]
		public void SetDebugTextChar(ICharObject charObject)
		{
		}
	}
}
