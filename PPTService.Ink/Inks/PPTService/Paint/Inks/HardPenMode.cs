using PPTService.Paint.Framework;
using PPTService.Paint.Framework.Settings;
using PPTService.Paint.Settings;

namespace PPTService.Paint.Inks
{
	public class HardPenMode : PenMode
	{
		protected internal override void AddInkIntoSlide(DynamicInk dynamicInk)
		{
			Element element = dynamicInk.AsStatic();
			if (!string.IsNullOrWhiteSpace(dynamicInk.InkId))
			{
				((IIdDefinition)element).SetId(dynamicInk.InkId);
			}
			CurrentSlide.Add(element);
			InsertAddingOperation(element);
		}

		protected override DynamicInk CreateDynamicInk()
		{
			InkBoardSettings settings = CurrentSlide.GetSettings<InkBoardSettings>();
			HardPenDynamicInk hardPenDynamicInk = new HardPenDynamicInk(CurrentSlide, settings);
			hardPenDynamicInk.DrawingAttributes.FitToCurve = false;
			return hardPenDynamicInk;
		}
	}
}
