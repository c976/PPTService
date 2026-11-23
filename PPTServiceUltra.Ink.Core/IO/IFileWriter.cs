using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTServiceUltra.Ink.Core.IO
{
	
	internal interface IFileWriter
	{
		Task AppendAllLinesAsync(IEnumerable<string> contentList);
	}
}
