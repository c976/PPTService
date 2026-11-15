using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.IO
{
	internal class DefaultLogValuesFormatter : ILogValuesFormatter
	{
		
		public string Format([In] ref LogValueContext context)
		{
			return context.ToString() ?? "";
		}

		
		string ILogValuesFormatter.Format([In] ref LogValueContext context)
		{
			return Format(ref context);
		}
	}
}
