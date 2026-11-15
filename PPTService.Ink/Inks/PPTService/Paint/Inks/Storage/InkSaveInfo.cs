using PPTService.Paint.Framework;
using PPTService.Ink.Core.Storage;
using PPTService.Ink.Core.Storage.Standard;

namespace PPTService.Paint.Inks.Storage
{
	public abstract class InkSaveInfo : ElementSaveInfo
	{
		[SaveInfoMember("InkType")]
		public string Type { get; set; }

		[SaveInfoMember("ForegroundColor")]
		public Color Foreground { get; set; }

		[SaveInfoMember("Thickness")]
		public double Thickness { get; set; }

		[SaveInfoMember("Points")]
		public StylusPointArray Points { get; set; }

		internal void SetType(string type)
		{
			Type = type;
		}
	}
}
