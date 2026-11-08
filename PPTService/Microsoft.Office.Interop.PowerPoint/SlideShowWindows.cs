using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[CompilerGenerated]
	[Guid("91493456-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface SlideShowWindows : Collection
	{
		void _VtblGap1_2();

		[DispId(11)]
		int Count
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(11)]
			get;
		}

		void _VtblGap2_2();

		[DispId(0)]
		SlideShowWindow this[[In] int Index]
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(0)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}
	}
}
