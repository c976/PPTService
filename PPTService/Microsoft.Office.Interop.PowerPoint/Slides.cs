using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[CompilerGenerated]
	[DefaultMember("Item")]
	[Guid("91493469-5A91-11CF-8700-00AA0060263B")]
	[TypeIdentifier]
	public interface Slides : Collection
	{
		void _VtblGap1_2();

		[DispId(11)]
		int Count
		{
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[DispId(11)]
			get;
		}
	}
}
