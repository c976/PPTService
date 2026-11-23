using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTServiceUltra.Ink.Core.Cli.Utils;

namespace PPTServiceUltra.Ink.Core.Cli
{
	
	
	public class CommandLineAsyncHandlerBuilder : ICommandLineAsyncHandlerBuilder, ICommandLineHandlerBuilder
	{
		public CommandLine CommandLine { get; }

		internal CommandLineAsyncHandlerBuilder(CommandLine commandLine)
		{
			if (commandLine == null)
			{
				throw new ArgumentNullException("commandLine");
			}
			CommandLine = commandLine;
		}

		public Task<int> RunAsync()
		{
			return CommandLineRunner.RunCoreAsync(CommandLine);
		}
	}
}
