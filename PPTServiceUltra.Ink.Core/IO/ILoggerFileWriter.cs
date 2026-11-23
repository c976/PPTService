using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	public interface ILoggerFileWriter
	{
		Task LogAsync(IEnumerable<LogValueContext> contextList);
	}
}
