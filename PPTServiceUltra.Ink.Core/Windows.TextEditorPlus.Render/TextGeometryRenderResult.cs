using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace PPTServiceUltra.Ink.Core.Windows.TextEditorPlus.Render
{
	
	
	
	internal struct TextGeometryRenderResult
	{
		public Geometry TextGeometry { get; }

		public Size TextGeometrySize { get; }

		
		
		public TextCharGlyphData TextCharGlyphData
		{
			
			get;
		}

		public TextGeometryRenderResult(Geometry textGeometry, Size textGeometrySize,  TextCharGlyphData textCharGlyphData)
		{
			TextGeometry = textGeometry;
			TextGeometrySize = textGeometrySize;
			TextCharGlyphData = textCharGlyphData;
		}

		public void Deconstruct(out Geometry textGeometry, out Size textGeometrySize,  out TextCharGlyphData textCharGlyphData)
		{
			textGeometry = TextGeometry;
			textGeometrySize = TextGeometrySize;
			textCharGlyphData = TextCharGlyphData;
		}

		public static implicit operator TextGeometryRenderResult( ValueTuple<Geometry, Size, TextCharGlyphData> result)
		{
			return new TextGeometryRenderResult(result.Item1, result.Item2, result.Item3);
		}

		
		
		public static implicit operator ValueTuple<Geometry, Size, TextCharGlyphData>(TextGeometryRenderResult result)
		{
			return new ValueTuple<Geometry, Size, TextCharGlyphData>(result.TextGeometry, result.TextGeometrySize, result.TextCharGlyphData);
		}
	}
}
