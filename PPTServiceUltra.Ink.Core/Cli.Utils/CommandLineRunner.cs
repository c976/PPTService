using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTServiceUltra.Ink.Core.Cli.Core;
using PPTServiceUltra.Ink.Core.Cli.StateMachine;

namespace PPTServiceUltra.Ink.Core.Cli.Utils
{
	internal static class CommandLineRunner
	{
		
		internal static Task<int> RunCoreAsync(CommandLine commandLine)
		{
			string text = CommandLineHelpers.FindPossibleVerb(commandLine);
			Dictionary<CommandLineFilterMatch, ICommandLineFilter> dictionary = new Dictionary<CommandLineFilterMatch, ICommandLineFilter>();
			CommandLineFilterContext commandLineFilterContext = new CommandLineFilterContext(commandLine, text);
			foreach (CommandLineFilterMatch filterMatch in commandLine.FilterMatchList)
			{
				ICommandLineFilter commandLineFilter = (dictionary[filterMatch] = filterMatch.FilterCreator());
				commandLineFilter.Filter(commandLineFilterContext);
				if (commandLineFilterContext.AreFurtherHandlersSuppressed)
				{
					return Task.FromResult(commandLineFilterContext.ExitCode);
				}
			}
			using (IEnumerator<CommandLineTypeMatchResult<Task<int>>> enumerator2 = new HandleVerbStateMachine<Task<int>>(commandLine.VerbMatchList).Find(text).GetEnumerator())
			{
				if (enumerator2.MoveNext())
				{
					return enumerator2.Current.Handler();
				}
			}
			foreach (CommandLineFilterMatch filterMatch2 in commandLine.FilterMatchList)
			{
				ICommandLineFilter value;
				(dictionary.TryGetValue(filterMatch2, out value) ? value : filterMatch2.FilterCreator()).PostFilter(commandLineFilterContext);
				if (commandLineFilterContext.AreFurtherHandlersSuppressed)
				{
					return Task.FromResult(commandLineFilterContext.ExitCode);
				}
			}
			CommandLineHelpers.ThrowIfVerbNotMatchedAsync(text);
			return Task.FromResult(0);
		}
	}
}
