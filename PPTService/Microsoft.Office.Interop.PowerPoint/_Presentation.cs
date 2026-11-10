using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("9149349D-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface _Presentation
	{
		void _VtblGap1_10();

		[DispId(2011)]
		Slides Slides
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2011)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		void _VtblGap2_3();

		[DispId(2015)]
		SlideShowSettings SlideShowSettings
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2015)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		void _VtblGap3_8();

		[DispId(2024)]
		string FullName
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2024)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}
	}
}
