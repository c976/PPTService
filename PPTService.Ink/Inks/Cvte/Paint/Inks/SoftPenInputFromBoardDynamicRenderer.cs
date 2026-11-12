namespace Cvte.Paint.Inks
{
	public class SoftPenInputFromBoardDynamicRenderer : HardPenInputFromBoardDynamicRenderer
	{
		public SoftPenInputFromBoardDynamicRenderer(InkCanvas inkCanvas)
			: base(inkCanvas)
		{
		}

		public override InkStrokeTipBuilder GetInkStrokeTipBuilder()
		{
			return new SoftPenInkStrokeTipBuilder();
		}
	}
}
