using System.Runtime.CompilerServices;

namespace PPTServiceUltra.Ink.Core.Cli
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
