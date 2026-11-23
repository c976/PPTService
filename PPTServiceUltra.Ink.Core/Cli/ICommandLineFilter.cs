using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Cli
{
	
	public interface ICommandLineFilter
	{
		void Filter(ICommandLineFilterContext context);

		void PostFilter(ICommandLineFilterContext context);
	}
}
