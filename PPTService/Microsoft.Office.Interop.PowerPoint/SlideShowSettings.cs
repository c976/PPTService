using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[CompilerGenerated]
	[Guid("9149345A-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface SlideShowSettings
	{
		void _VtblGap1_4();

		[DispId(2005)]
		int StartingSlide
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2005)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2005)]
			[param: In]
			set;
		}

		[DispId(2006)]
		int EndingSlide
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2006)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2006)]
			[param: In]
			set;
		}

		void _VtblGap2_5();

		[DispId(2010)]
		PpSlideShowType ShowType
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2010)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2010)]
			[param: In]
			set;
		}

		void _VtblGap3_6();

		[DispId(2014)]
		PpSlideShowRangeType RangeType
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2014)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2014)]
			[param: In]
			set;
		}
	}
}
