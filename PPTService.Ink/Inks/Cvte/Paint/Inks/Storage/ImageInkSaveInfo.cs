using Cvte.Paint.Framework;
using Cvte.Storage;
using Cvte.Storage.Standard;

namespace Cvte.Paint.Inks.Storage
{
	[SaveInfoContract("ImageInk", Description = "图案笔")]
	public class ImageInkSaveInfo : ElementSaveInfo
	{
		[SaveInfoMember("Points", IsRequired = true)]
		public StylusPointArray Points { get; set; }

		[SaveInfoMember("Source", IsRequired = true, Description = "笔迹图片资源路径")]
		public Uri Source { get; set; }

		[SaveInfoMember("Scale", Description = "笔迹图片的缩放笔")]
		public double Scale { get; set; }
	}
}
