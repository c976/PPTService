using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PPTService.Ink.Core.IO
{
	
	
	internal class LoggerFileWriter : ILoggerFileWriter
	{
		public IFileWriter FileWriter { get; }

		private Func<LogValueContext, bool> MatchFunc { get; }

		public ILogValuesFormatter LogValuesFormatter { get; }

		public LoggerFileWriter(IFileWriter fileWriter, Func<LogValueContext, bool> matchFunc, ILogValuesFormatter logValuesFormatter)
		{
			FileWriter = fileWriter;
			MatchFunc = matchFunc;
			LogValuesFormatter = logValuesFormatter;
		}

		public Task LogAsync(IEnumerable<LogValueContext> contextList)
		{
						IEnumerable<string> contentList = contextList.Where(MatchFunc).Select(context => LogValuesFormatter.Format(ref context));
			return FileWriter.AppendAllLinesAsync(contentList);
		}
	}
}
