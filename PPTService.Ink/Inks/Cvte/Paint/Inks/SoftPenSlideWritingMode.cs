using System;

namespace Cvte.Paint.Inks
{
	public class SoftPenSlideWritingMode : HardPenSlideWritingMode, IPenMode, IDisposable
	{
		protected override DynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new SoftPenSlideDynamicRenderer(inkCanvas);
		}
	}
}
