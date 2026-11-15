using System.Windows.Media;
using PPTService.Paint.Framework;

namespace PPTService.Paint.Inks
{
	internal class EraseOperationInfo
	{
		public Element Element { get; }

		public Geometry OldGeometry { get; set; }

		public Geometry NewGeometry { get; set; }

		public EraseOperationInfo(Element element)
		{
			Element = element;
		}
	}
}
