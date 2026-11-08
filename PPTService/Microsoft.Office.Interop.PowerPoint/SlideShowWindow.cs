using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[CompilerGenerated]
	[Guid("91493453-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface SlideShowWindow
	{
		void _VtblGap1_2();

		[DispId(2003)]
		SlideShowView View
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2003)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		[DispId(2004)]
		Presentation Presentation
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2004)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		void _VtblGap2_9();

		[DispId(2010)]
		int HWND
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2010)]
			get;
		}

		void _VtblGap3_1();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2012)]
		void Activate();
	}
}
