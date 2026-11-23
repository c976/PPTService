using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("91493442-5A91-11CF-8700-00AA0060263B")]
	[CoClass(typeof(object))]
	[TypeIdentifier]
	public interface Application : _Application, EApplication_Event
	{
	}
}
