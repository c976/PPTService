using System.Windows.Media;
using PPTServiceUltra.Paint.Framework;

namespace PPTServiceUltra.Paint.Inks
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
