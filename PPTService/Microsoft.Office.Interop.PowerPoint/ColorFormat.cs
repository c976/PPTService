using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[CompilerGenerated]
	[DefaultMember("RGB")]
	[Guid("91493452-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface ColorFormat
	{
		void _VtblGap1_3();

		[DispId(0)]
		int RGB
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(0)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(0)]
			[param: In]
			set;
		}
	}
}
