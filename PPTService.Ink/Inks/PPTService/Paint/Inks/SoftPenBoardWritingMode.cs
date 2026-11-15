using System;

namespace PPTService.Paint.Inks
{
	public class SoftPenBoardWritingMode : HardPenBoardWritingMode, IPenMode, IDisposable
	{
		protected override DynamicRenderer GetDynamicRenderer(InkCanvas inkCanvas)
		{
			return new SoftPenBoardDynamicRenderer(inkCanvas)
			{
				TransformFromBoard = TransformFromBoard
			};
		}
	}
}
