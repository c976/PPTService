using System;
using System.Runtime.CompilerServices;
using PPTService.Ink.Core.Cli.Utils;

namespace PPTService.Ink.Core.Cli
{
	
	
	public static class CommandLineFilters
	{
		public static CommandLineHandlerBuilder AddFilter< TFilter>(this ICommandLineHandlerBuilder builder) where TFilter : ICommandLineFilter, new()
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			return builder.AddFilterCore<TFilter>(null);
		}

		public static CommandLineHandlerBuilder AddFilter< TFilter>(this ICommandLineHandlerBuilder builder, ICommandLineOptionParser<TFilter> parser) where TFilter : ICommandLineFilter
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			if (parser == null)
			{
				throw new ArgumentNullException("parser");
			}
			return builder.AddFilterCore(parser);
		}

		public static CommandLineAsyncHandlerBuilder AddFilter< TFilter>(this ICommandLineAsyncHandlerBuilder builder) where TFilter : ICommandLineFilter, new()
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			return builder.AddFilterCore<TFilter>(null);
		}

		public static CommandLineAsyncHandlerBuilder AddFilter< TFilter>(this ICommandLineAsyncHandlerBuilder builder,  ICommandLineOptionParser<TFilter> parser = null) where TFilter : ICommandLineFilter
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			if (parser == null)
			{
				throw new ArgumentNullException("parser");
			}
			return builder.AddFilterCore(parser);
		}

		private static CommandLineHandlerBuilder AddFilterCore< TFilter>(this ICommandLineHandlerBuilder builder,  ICommandLineOptionParser<TFilter> parser) where TFilter : ICommandLineFilter
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			AddFilterCore(builder.CommandLine, parser);
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

		private static CommandLineAsyncHandlerBuilder AddFilterCore< TFilter>(this ICommandLineAsyncHandlerBuilder builder,  ICommandLineOptionParser<TFilter> parser) where TFilter : ICommandLineFilter
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			AddFilterCore(builder.CommandLine, parser);
			CommandLineAsyncHandlerBuilder commandLineAsyncHandlerBuilder = builder as CommandLineAsyncHandlerBuilder;
			if (commandLineAsyncHandlerBuilder != null)
			{
				return commandLineAsyncHandlerBuilder;
			}
			throw new NotSupportedException("接口 ICommandLineHandlerBuilder 不支持第三方实现。");
		}

		private static void AddFilterCore< TFilter>(CommandLine commandLine,  ICommandLineOptionParser<TFilter> parser) where TFilter : ICommandLineFilter
		{
			commandLine.AddMatch(CommandLineHelpers.MatchFilter(commandLine, parser));
		}
	}
}
