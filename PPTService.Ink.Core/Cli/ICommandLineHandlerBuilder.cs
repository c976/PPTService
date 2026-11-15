using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli
{
	
	public interface ICommandLineHandlerBuilder
	{
		CommandLine CommandLine { get; }
	}
}
