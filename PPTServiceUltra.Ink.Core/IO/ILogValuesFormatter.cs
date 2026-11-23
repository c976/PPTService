using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	public interface ILogValuesFormatter
	{
		string Format([In] ref LogValueContext context);
	}
}
