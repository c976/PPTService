using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("91493471-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface SlideShowTransition
	{
		void _VtblGap1_2();

		[DispId(2003)]
		MsoTriState AdvanceOnClick
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2003)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2003)]
			[param: In]
			set;
		}
	}
}
