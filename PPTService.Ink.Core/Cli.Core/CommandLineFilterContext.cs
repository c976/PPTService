using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using PPTService.Ink.Core.Cli.StateMachine;

namespace PPTService.Ink.Core.Cli.Core
{
	
	
	internal class CommandLineFilterContext : ICommandLineFilterContext
	{
		private volatile int _areFurtherHandlersSuppressed;

		public CommandLine CommandLine { get; }

		
		
		public string Verb
		{
			
			get;
		}

		public bool AreFurtherHandlersSuppressed => _areFurtherHandlersSuppressed == 1;

		public int ExitCode { get; private set; }

		internal CommandLineFilterContext(CommandLine commandLine,  string verb)
		{
			if (commandLine == null)
			{
				throw new ArgumentNullException("commandLine");
			}
			CommandLine = commandLine;
			Verb = verb;
		}

		internal IEnumerable<Type> EnumerateRelatedTypes()
		{
			return CommandLine.FilterMatchList.Select( (CommandLineFilterMatch x) => x.FilterType).Concat(CommandLine.VerbMatchList.Select( (CommandLineTypeMatcher<Task<int>> x) => x.VerbType));
		}

		
		internal Type GetVerbType()
		{
			using (IEnumerator<CommandLineTypeMatchResult<Task<int>>> enumerator = new HandleVerbStateMachine<Task<int>>(CommandLine.VerbMatchList).Find(Verb).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return enumerator.Current.VerbType;
				}
			}
			return null;
		}

		public void SuppressFurtherHandlers(int exitCode = 0)
		{
			if (Interlocked.Increment(ref _areFurtherHandlersSuppressed) == 1)
			{
				ExitCode = exitCode;
				return;
			}
			throw new InvalidOperationException("在一个命令行执行上下文中，仅允许标记一次拦截后续执行。");
		}
	}
}
