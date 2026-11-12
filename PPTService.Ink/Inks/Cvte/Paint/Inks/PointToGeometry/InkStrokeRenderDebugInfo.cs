using System;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Cvte.Diagnostics.Annotations;

namespace Cvte.Paint.Inks.PointToGeometry
{
	public struct InkStrokeRenderDebugInfo
	{
		public DrawingContext DebugDrawingContext { get; }

		public double FeedbackSize { get; }

		public InkStrokeRenderDebugInfo([NotNull] DrawingContext debugDrawingContext, double feedbackSize)
		{
			if (debugDrawingContext == null)
			{
				throw new ArgumentNullException("debugDrawingContext");
			}
			DebugDrawingContext = debugDrawingContext;
			FeedbackSize = feedbackSize;
		}
	}
}
