using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("91493459-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface SlideShowView
	{
		void _VtblGap1_4();

		[DispId(2005)]
		PpSlideShowPointerType PointerType
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2005)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2005)]
			[param: In]
			set;
		}

		void _VtblGap2_9();

		[DispId(2012)]
		ColorFormat PointerColor
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2012)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		void _VtblGap3_6();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2019)]
		void Next();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2020)]
		void Previous();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2021)]
		void GotoSlide([In] int Index, [In] MsoTriState ResetSlide = MsoTriState.msoTrue);

		void _VtblGap4_3();

		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(2025)]
		void Exit();

		void _VtblGap5_1();

		[DispId(2027)]
		int CurrentShowPosition
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2027)]
			get;
		}
	}
}
