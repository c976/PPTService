using System.Windows.Documents;

namespace PPTService.Paint.Framework
{
	public static class SlideTreeExtensions
	{
		public static AdornerLayer GetInnerLayer(this Slide slide)
		{
			return AdornerLayer.GetAdornerLayer(slide.Canvas);
		}

		public static AdornerLayer GetOuterLayer(this Slide slide)
		{
			return AdornerLayer.GetAdornerLayer(slide);
		}
	}
}
