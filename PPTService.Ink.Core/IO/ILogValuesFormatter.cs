using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.IO
{
	
	public interface ILogValuesFormatter
	{
		string Format([In] ref LogValueContext context);
	}
}
