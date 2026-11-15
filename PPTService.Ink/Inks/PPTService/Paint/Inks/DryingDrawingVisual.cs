using System;
using System.Collections.Generic;
using System.Windows.Media;
using PPTService.Ink.Core.Diagnostics.Annotations;

namespace PPTService.Paint.Inks
{
	public class DryingDrawingVisual
	{
		public ContainerVisual ContainerVisual { get; }

		public List<DrawingVisual> DryingDrawingVisualList { get; }

		public DryingDrawingVisual([NotNull] ContainerVisual containerVisual, [NotNull] List<DrawingVisual> dryingDrawingVisualList)
		{
			if (containerVisual == null)
			{
				throw new ArgumentNullException("containerVisual");
			}
			if (dryingDrawingVisualList == null)
			{
				throw new ArgumentNullException("dryingDrawingVisualList");
			}
			ContainerVisual = containerVisual;
			DryingDrawingVisualList = dryingDrawingVisualList;
		}
	}
}
