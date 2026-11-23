using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("91493442-5A91-11CF-8700-00AA0060263B")]
	[DefaultMember("Name")]
	[TypeIdentifier]
	public interface _Application
	{
		[DispId(2001)]
		Presentations Presentations
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2001)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		void _VtblGap1_3();

		[DispId(2005)]
		Presentation ActivePresentation
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2005)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		[DispId(2006)]
		SlideShowWindows SlideShowWindows
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2006)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		void _VtblGap2_1();

		[DispId(2008)]
		string Path
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2008)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		[DispId(0)]
		string Name
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(0)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		[DispId(2009)]
		string Caption
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2009)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2009)]
			[param: In]
			[param: MarshalAs(UnmanagedType.BStr)]
			set;
		}

		void _VtblGap3_4();

		[DispId(2014)]
		string Version
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2014)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		void _VtblGap4_22();

		[DispId(2031)]
		int HWND
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2031)]
			get;
		}
	}
}
