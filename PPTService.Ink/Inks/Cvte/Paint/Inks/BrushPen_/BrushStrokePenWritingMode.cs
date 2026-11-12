using System;

namespace Cvte.Paint.Inks.BrushPen_
{
	[Obsolete("EN3的软笔，暂时没有完全实现")]
	internal class BrushStrokePenWritingMode : SoftPenSlideWritingMode
	{
		protected override DynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new BrushStrokePenDynamicRenderer(inkCanvas);
		}

		protected override void InkCanvas_StrokesCollected(object sender, InkSynchronizer e)
		{
		}
	}
}
