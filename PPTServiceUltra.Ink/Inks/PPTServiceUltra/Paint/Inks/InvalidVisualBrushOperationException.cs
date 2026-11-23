using System;
using System.Collections.Generic;
using System.Windows.Media;
using PPTServiceUltra.Ink.Core.Diagnostics.Annotations;

namespace PPTServiceUltra.Paint.Inks
{
	public class InvalidVisualBrushOperationException : InvalidOperationException
	{
		[PublicAPI]
		public List<VisualBrush> VisualBrushList { get; }

		public InvalidVisualBrushOperationException(string message, List<VisualBrush> visualBrushList)
			: base(message)
		{
			VisualBrushList = visualBrushList;
		}
	}
}
