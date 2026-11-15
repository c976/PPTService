using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli
{
	
	public interface ICommandLineFilter
	{
		void Filter(ICommandLineFilterContext context);

		void PostFilter(ICommandLineFilterContext context);
	}
}
