using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("914934C2-5A91-11CF-8700-00AA0060263B")]
	[ComEventInterface(typeof(EApplication), typeof(EApplication))]
	public interface EApplication_Event
	{
		void _VtblGap1_20();

		event EApplication_SlideShowBeginEventHandler SlideShowBegin;

		void _VtblGap2_2();

		event EApplication_SlideShowNextSlideEventHandler SlideShowNextSlide;

		event EApplication_SlideShowEndEventHandler SlideShowEnd;

		void _VtblGap3_32();

		event EApplication_PresentationCloseFinalEventHandler PresentationCloseFinal;
	}
}
