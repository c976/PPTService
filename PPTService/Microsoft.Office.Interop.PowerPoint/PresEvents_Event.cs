using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Interop.PowerPoint
{
	[ComImport]
	[Guid("91493463-5A91-11CF-8700-00AA0060263B")]
	[ComEventInterface(typeof(PresEvents), typeof(PresEvents))]
	public interface PresEvents_Event
	{
	}
}
