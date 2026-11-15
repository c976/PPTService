using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli
{
	
	public interface ICommandLineFilterContext
	{
		CommandLine CommandLine { get; }

		
		string Verb
		{
			
			get;
		}

		void SuppressFurtherHandlers(int exitCode = 0);
	}
}
