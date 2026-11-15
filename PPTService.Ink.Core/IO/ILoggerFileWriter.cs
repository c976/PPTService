using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.IO
{
	
	public interface ILoggerFileWriter
	{
		Task LogAsync(IEnumerable<LogValueContext> contextList);
	}
}
