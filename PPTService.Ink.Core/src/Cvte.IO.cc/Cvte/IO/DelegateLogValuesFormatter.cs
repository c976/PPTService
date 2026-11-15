using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.IO
{
	
	
	public class DelegateLogValuesFormatter : ILogValuesFormatter
	{
		private Func<LogValueContext, string> Formatter { get; }

		public DelegateLogValuesFormatter(Func<LogValueContext, string> formatter)
		{
			Formatter = formatter;
		}

		public string Format([In] ref LogValueContext context)
		{
			return Formatter(context);
		}

		string ILogValuesFormatter.Format([In] ref LogValueContext context)
		{
			return Format(ref context);
		}
	}
}
