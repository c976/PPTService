using Cvte.Paint.Framework;
using Cvte.Storage;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Inks.Storage
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
