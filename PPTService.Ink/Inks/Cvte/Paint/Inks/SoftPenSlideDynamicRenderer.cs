namespace Cvte.Paint.Inks
{
	public class SoftPenSlideDynamicRenderer : DynamicRenderer
	{
		public SoftPenSlideDynamicRenderer(InkCanvas inkCanvas)
			: base(inkCanvas)
		{
		}

		public override InkStrokeTipBuilder GetInkStrokeTipBuilder()
		{
			return new SoftPenInkStrokeTipBuilder();
		}
	}
}
