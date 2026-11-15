using System;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Cli.Utils;

namespace PPTService.Ink.Core.Cli
{
	
	
	public class CommandLineHandlerBuilder : ICommandLineHandlerBuilder
	{
		public CommandLine CommandLine { get; }

		internal CommandLineHandlerBuilder(CommandLine commandLine)
		{
			if (commandLine == null)
			{
				throw new ArgumentNullException("commandLine");
			}
			CommandLine = commandLine;
		}

		public int Run()
		{
			return CommandLineRunner.RunCoreAsync(CommandLine).Result;
		}
	}
}
