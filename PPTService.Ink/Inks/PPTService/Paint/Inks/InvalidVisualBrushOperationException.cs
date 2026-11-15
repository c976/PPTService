using System;
using System.Collections.Generic;
using System.Windows.Media;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Paint.Inks
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
