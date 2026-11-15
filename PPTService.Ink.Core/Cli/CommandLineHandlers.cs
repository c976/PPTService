using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTService.Ink.Core.Cli.StateMachine;
using PPTService.Ink.Core.Cli.Utils;

namespace PPTService.Ink.Core.Cli
{
	
	
	public static class CommandLineHandlers
	{
		public static CommandLineHandlerBuilder AddHandler< TVerb>(this ICommandLineHandlerBuilder builder, Action<TVerb> handler,  ICommandLineOptionParser<TVerb> parser = null)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			builder.CommandLine.AddMatch<TVerb>(new CommandLineTypeMatcher<Task<int>>(typeof(TVerb),   (string verb) => CommandLineHelpers.MatchWithHandler(builder.CommandLine, verb,   (TVerb options) => CommandLineHelpers.Invoke(handler, options), parser)));
			CommandLineHandlerBuilder commandLineHandlerBuilder = builder as CommandLineHandlerBuilder;
			if (commandLineHandlerBuilder == null)
			{
				CommandLine commandLine = builder as CommandLine;
				if (commandLine != null)
				{
					return new CommandLineHandlerBuilder(commandLine);
				}
				throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
			}
			return commandLineHandlerBuilder;
		}

		public static CommandLineHandlerBuilder AddHandler< TVerb>(this ICommandLineHandlerBuilder builder, Func<TVerb, int> handler,  ICommandLineOptionParser<TVerb> parser = null)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			builder.CommandLine.AddMatch<TVerb>(new CommandLineTypeMatcher<Task<int>>(typeof(TVerb),   (string verb) => CommandLineHelpers.MatchWithHandler(builder.CommandLine, verb,   (TVerb options) => CommandLineHelpers.Invoke(handler, options), parser)));
			CommandLineHandlerBuilder commandLineHandlerBuilder = builder as CommandLineHandlerBuilder;
			if (commandLineHandlerBuilder == null)
			{
				CommandLine commandLine = builder as CommandLine;
				if (commandLine != null)
				{
					return new CommandLineHandlerBuilder(commandLine);
				}
				throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
			}
			return commandLineHandlerBuilder;
		}

		public static CommandLineAsyncHandlerBuilder AddHandler< TVerb>(this ICommandLineHandlerBuilder builder, Func<TVerb, Task> handler,  ICommandLineOptionParser<TVerb> parser = null)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			CommandLine commandLine = builder.CommandLine;
			new CommandLineAsyncHandlerBuilder(commandLine);
			builder.CommandLine.AddMatch<TVerb>(new CommandLineTypeMatcher<Task<int>>(typeof(TVerb),   (string verb) => CommandLineHelpers.MatchWithHandler(commandLine, verb,   (TVerb options) => CommandLineHelpers.Invoke(handler, options), parser)));
			if (!(builder is CommandLineHandlerBuilder))
			{
				if (builder is CommandLine)
				{
					return new CommandLineAsyncHandlerBuilder(commandLine);
				}
				throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
			}
			return new CommandLineAsyncHandlerBuilder(commandLine);
		}

		public static CommandLineAsyncHandlerBuilder AddHandler< TVerb>(this ICommandLineHandlerBuilder builder, Func<TVerb, Task<int>> handler,  ICommandLineOptionParser<TVerb> parser = null)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			CommandLine commandLine = builder.CommandLine;
			new CommandLineAsyncHandlerBuilder(commandLine);
			builder.CommandLine.AddMatch<TVerb>(new CommandLineTypeMatcher<Task<int>>(typeof(TVerb),   (string verb) => CommandLineHelpers.MatchWithHandler(commandLine, verb,   (TVerb options) => CommandLineHelpers.Invoke(handler, options), parser)));
			if (!(builder is CommandLineHandlerBuilder))
			{
				if (builder is CommandLine)
				{
					return new CommandLineAsyncHandlerBuilder(commandLine);
				}
				throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
			}
			return new CommandLineAsyncHandlerBuilder(commandLine);
		}

		public static CommandLineAsyncHandlerBuilder AddHandler< TVerb>(this ICommandLineAsyncHandlerBuilder builder, Action<TVerb> handler,  ICommandLineOptionParser<TVerb> parser = null)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			builder.CommandLine.AddMatch<TVerb>(new CommandLineTypeMatcher<Task<int>>(typeof(TVerb),   (string verb) => CommandLineHelpers.MatchWithHandler(builder.CommandLine, verb,   (TVerb options) => CommandLineHelpers.Invoke(handler, options), parser)));
			CommandLineAsyncHandlerBuilder commandLineAsyncHandlerBuilder = builder as CommandLineAsyncHandlerBuilder;
			if (commandLineAsyncHandlerBuilder != null)
			{
				return commandLineAsyncHandlerBuilder;
			}
			throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
		}

		public static CommandLineAsyncHandlerBuilder AddHandler< TVerb>(this ICommandLineAsyncHandlerBuilder builder, Func<TVerb, int> handler,  ICommandLineOptionParser<TVerb> parser = null)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			builder.CommandLine.AddMatch<TVerb>(new CommandLineTypeMatcher<Task<int>>(typeof(TVerb),   (string verb) => CommandLineHelpers.MatchWithHandler(builder.CommandLine, verb,   (TVerb options) => CommandLineHelpers.Invoke(handler, options), parser)));
			CommandLineAsyncHandlerBuilder commandLineAsyncHandlerBuilder = builder as CommandLineAsyncHandlerBuilder;
			if (commandLineAsyncHandlerBuilder != null)
			{
				return commandLineAsyncHandlerBuilder;
			}
			throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
		}

		public static CommandLineAsyncHandlerBuilder AddHandler< TVerb>(this ICommandLineAsyncHandlerBuilder builder, Func<TVerb, Task> handler,  ICommandLineOptionParser<TVerb> parser = null)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			builder.CommandLine.AddMatch<TVerb>(new CommandLineTypeMatcher<Task<int>>(typeof(TVerb),   (string verb) => CommandLineHelpers.MatchWithHandler(builder.CommandLine, verb,   (TVerb options) => CommandLineHelpers.Invoke(handler, options), parser)));
			CommandLineAsyncHandlerBuilder commandLineAsyncHandlerBuilder = builder as CommandLineAsyncHandlerBuilder;
			if (commandLineAsyncHandlerBuilder != null)
			{
				return commandLineAsyncHandlerBuilder;
			}
			throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
		}

		public static CommandLineAsyncHandlerBuilder AddHandler< TVerb>(this ICommandLineAsyncHandlerBuilder builder, Func<TVerb, Task<int>> handler,  ICommandLineOptionParser<TVerb> parser = null)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			builder.CommandLine.AddMatch<TVerb>(new CommandLineTypeMatcher<Task<int>>(typeof(TVerb),   (string verb) => CommandLineHelpers.MatchWithHandler(builder.CommandLine, verb,   (TVerb options) => CommandLineHelpers.Invoke(handler, options), parser)));
			CommandLineAsyncHandlerBuilder commandLineAsyncHandlerBuilder = builder as CommandLineAsyncHandlerBuilder;
			if (commandLineAsyncHandlerBuilder != null)
			{
				return commandLineAsyncHandlerBuilder;
			}
			throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
		}
	}
}
