using System;
using Cvte.Paint.Framework;

namespace Cvte.Paint.Inks
{
	public class SoftPenMode : PenMode
	{
		protected internal override void AddInkIntoSlide(DynamicInk dynamicInk)
		{
			Ink ink = (Ink)dynamicInk.AsStatic();
			((IIdDefinition)ink).SetId(dynamicInk.InkId);
			CurrentSlide.Add(ink);
			dynamicInk.Ink = ink;
			InsertAddingOperation(ink);
		}

		protected override DynamicInk CreateDynamicInk()
		{
			return new SoftPenDynamicInk(CurrentSlide, InkBoardSettings)
			{
				InkId = Guid.NewGuid().ToString("N")
			};
		}
	}
}
