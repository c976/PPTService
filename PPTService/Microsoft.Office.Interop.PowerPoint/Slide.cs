using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("9149346A-5A91-11CF-8700-00AA0060263B")]
	[CoClass(typeof(object))]
	[TypeIdentifier]
	public interface Slide : _Slide, SldEvents_Event
	{
	}
}
