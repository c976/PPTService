using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("9149346C-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface _Master
	{
		void _VtblGap1_10();

		[DispId(2009)]
		float Height
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2009)]
			get;
		}

		[DispId(2010)]
		float Width
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(2010)]
			get;
		}
	}
}
