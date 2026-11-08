using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[CompilerGenerated]
	[Guid("914934C2-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface EApplication
	{
		void _VtblGap1_10();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2011)]
		void SlideShowBegin([In][MarshalAs(UnmanagedType.Interface)] SlideShowWindow Wn);

		void _VtblGap2_1();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2013)]
		void SlideShowNextSlide([In][MarshalAs(UnmanagedType.Interface)] SlideShowWindow Wn);

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2014)]
		void SlideShowEnd([In][MarshalAs(UnmanagedType.Interface)] Presentation Pres);

		void _VtblGap3_16();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2031)]
		void PresentationCloseFinal([In][MarshalAs(UnmanagedType.Interface)] Presentation Pres);
	}
}
