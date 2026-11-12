using System;
using Cvte.Paint.Framework;
using Cvte.Paint.Framework.Settings;
using Cvte.Paint.Settings;

namespace Cvte.Paint.Inks
{
	public class MagicPenMode : PenMode
	{
		public MagicPenMode()
		{
		}

		protected MagicPenMode(ModeSettings modeSettings)
			: base(modeSettings)
		{
		}

		protected internal override void AddInkIntoSlide(DynamicInk dynamicInk)
		{
			RoadDynamicInk roadDynamicInk = dynamicInk as RoadDynamicInk;
			if (roadDynamicInk != null && roadDynamicInk.StrokeType == InkType.River)
			{
				RiverStroke.IsDirectionInit = false;
			}
			Element element = dynamicInk.AsStatic();
			((IIdDefinition)element).SetId(dynamicInk.InkId);
			CurrentSlide.Add(element);
			InsertAddingOperation(element);
		}

		protected override DynamicInk CreateDynamicInk()
		{
			return CreateDynamicInkBySetting(CurrentSlide.GetSettings<InkBoardSettings>());
		}

		protected DynamicInk CreateDynamicInkBySetting(InkBoardSettings settings)
		{
			double inkScale = settings.InkScale;
			if (settings.InkType == InkType.Image)
			{
				return new ImageDynamicInk(CurrentSlide, settings, inkScale)
				{
					InkId = Guid.NewGuid().ToString("N")
				};
			}
			MagicStrokeInfo info;
			switch (settings.InkType)
			{
			case InkType.Railway:
				info = new MagicStrokeInfo(settings.InkType, typeof(RailwayStroke), inkScale);
				break;
			case InkType.River:
				info = new MagicStrokeInfo(settings.InkType, typeof(RiverStroke), inkScale);
				RiverStroke.IsDirectionInit = false;
				break;
			default:
				info = new MagicStrokeInfo(settings.InkType, typeof(GreatwallStroke), inkScale);
				break;
			}
			return new RoadDynamicInk(CurrentSlide, settings, info)
			{
				InkId = Guid.NewGuid().ToString("N")
			};
		}
	}
}
