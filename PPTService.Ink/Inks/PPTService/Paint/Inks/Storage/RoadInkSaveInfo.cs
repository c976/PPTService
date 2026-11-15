using PPTService.Paint.Framework;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.Standard;

namespace PPTService.Paint.Inks.Storage
{
	[SaveInfoContract("RoadInk", Description = "图案笔")]
	public class RoadInkSaveInfo : ElementSaveInfo
	{
		[SaveInfoMember("Points", IsRequired = true)]
		public StylusPointArray Points { get; set; }

		[SaveInfoMember("StrokeType", IsRequired = true, Description = "笔迹类型")]
		public InkType StrokeType { get; set; }
	}
}
