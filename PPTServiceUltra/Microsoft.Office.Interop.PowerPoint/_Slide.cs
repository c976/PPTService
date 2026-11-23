using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("9149346A-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface _Slide
	{
		void _VtblGap1_4();

		[DispId(2005)]
		SlideShowTransition SlideShowTransition
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2005)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		void _VtblGap2_15();

		[DispId(2018)]
		int SlideIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2018)]
			get;
		}

		void _VtblGap3_6();

		[DispId(2023)]
		Master Master
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2023)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		void _VtblGap4_1();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2025)]
		void Export([In][MarshalAs(UnmanagedType.BStr)] string FileName, [In][MarshalAs(UnmanagedType.BStr)] string FilterName, [In] int ScaleWidth = 0, [In] int ScaleHeight = 0);
	}
}
