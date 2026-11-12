namespace Cvte.Paint.Inks
{
	public class SoftPenBoardDynamicRenderer : HardPenBoardDynamicRenderer
	{
		public SoftPenBoardDynamicRenderer(InkCanvas inkCanvas)
			: base(inkCanvas)
		{
		}

		public override InkStrokeTipBuilder GetInkStrokeTipBuilder()
		{
			return new SoftPenInkStrokeTipBuilder();
		}
	}
}
