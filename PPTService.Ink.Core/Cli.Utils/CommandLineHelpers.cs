using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PPTService.Ink.Core.Cli.Core;
using PPTService.Ink.Core.Cli.Parsers;
using PPTService.Ink.Core.Cli.StateMachine;

namespace PPTService.Ink.Core.Cli.Utils
{
	
	
	internal static class CommandLineHelpers
	{
		internal static ICommandLineOptionParser<T> FindParser< T>()
		{
			ICommandLineOptionParser<T> commandLineOptionParser = null;
			Type typeFromHandle = typeof(T);
			Type type = typeFromHandle.Assembly.GetType(typeFromHandle.FullName + "Parser", false, false);
			if (type != null)
			{
				commandLineOptionParser = Activator.CreateInstance(type) as ICommandLineOptionParser<T>;
			}
			return commandLineOptionParser ?? RuntimeCommandLineOptionParser<T>.Create();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		internal static string FindPossibleVerb(CommandLine commandLine)
		{
			KeyValuePair<string, SingleOptimizedStrings> keyValuePair = commandLine.FirstOrDefault();
			string key = keyValuePair.Key;
			SingleOptimizedStrings value = keyValuePair.Value;
			return (string.IsNullOrWhiteSpace(key) ? value : null)?.FirstOrDefault();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static CommandLineFilterMatch MatchFilter< TFilter>(CommandLine commandLine,  ICommandLineOptionParser<TFilter> parser) where TFilter : ICommandLineFilter
		{
			return new CommandLineFilterMatch(typeof(TFilter), () => commandLine.As(parser ?? FindParser<TFilter>()));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		internal static CommandLineTypeMatchResult<int> MatchWithHandler< TVerb>(CommandLine commandLine,  string possibleVerb, Action<TVerb> handler,  ICommandLineOptionParser<TVerb> parser)
		{
			return MatchWithHandler(commandLine, possibleVerb,  (TVerb verb) =>
			{
				handler(verb);
				return 0;
			}, parser);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		internal static CommandLineTypeMatchResult<int> MatchWithHandler< TVerb>(CommandLine commandLine,  string possibleVerb, Func<TVerb, int> handler,  ICommandLineOptionParser<TVerb> parser)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (parser == null)
			{
				parser = FindParser<TVerb>();
			}
			if (string.Equals(possibleVerb, parser.Verb, StringComparison.InvariantCultureIgnoreCase))
			{
				return new CommandLineTypeMatchResult<int>(VerbMatchingResult.Matched, typeof(TVerb), possibleVerb, () => handler(commandLine.As(parser)));
			}
			if (parser.Verb == null)
			{
				return new CommandLineTypeMatchResult<int>(VerbMatchingResult.FallbackMatched, typeof(TVerb), possibleVerb, () => handler(commandLine.As(parser)));
			}
			return new CommandLineTypeMatchResult<int>(VerbMatchingResult.NotMatch, typeof(TVerb), possibleVerb);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		internal static CommandLineTypeMatchResult<Task<int>> MatchWithHandler< TVerb>(CommandLine commandLine,  string possibleVerb, Func<TVerb, Task> handler,  ICommandLineOptionParser<TVerb> parser)
		{
			return MatchWithHandler(commandLine, possibleVerb,   async (TVerb verb) =>
			{
				await handler(verb).ConfigureAwait(false);
				return 0;
			}, parser);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		internal static CommandLineTypeMatchResult<Task<int>> MatchWithHandler< TVerb>(CommandLine commandLine,  string possibleVerb, Func<TVerb, Task<int>> handler,  ICommandLineOptionParser<TVerb> parser)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (parser == null)
			{
				parser = FindParser<TVerb>();
			}
			if (string.Equals(possibleVerb, parser.Verb, StringComparison.InvariantCultureIgnoreCase))
			{
				return new CommandLineTypeMatchResult<Task<int>>(VerbMatchingResult.Matched, typeof(TVerb), possibleVerb, () => handler(commandLine.As(parser)));
			}
			if (parser.Verb == null)
			{
				return new CommandLineTypeMatchResult<Task<int>>(VerbMatchingResult.FallbackMatched, typeof(TVerb), possibleVerb, () => handler(commandLine.As(parser)));
			}
			return new CommandLineTypeMatchResult<Task<int>>(VerbMatchingResult.NotMatch, typeof(TVerb), possibleVerb);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Task<int> Invoke< TVerb>(Action<TVerb> handler, TVerb options)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			handler(options);
			return Task.FromResult(0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Task<int> Invoke< TVerb>(Func<TVerb, int> handler, TVerb options)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			return Task.FromResult(handler(options));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static async Task<int> Invoke< TVerb>(Func<TVerb, Task> handler, TVerb options)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			await handler(options).ConfigureAwait(false);
			return 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static async Task<int> Invoke< TVerb>(Func<TVerb, Task<int>> handler, TVerb options)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			return await handler(options).ConfigureAwait(false);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		internal static int ThrowIfVerbNotMatched(string possibleVerb)
		{
			throw (possibleVerb == null) ? new ArgumentException("传入的命令行参数中没有发现谓词，但此方法规定的所有命令行类型都要求有谓词。", "possibleVerb") : new ArgumentException("没有任何命令行参数类型符合谓词 " + possibleVerb + "。", "possibleVerb");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		
		internal static int ThrowIfVerbNotMatchedAsync(string possibleVerb)
		{
			throw (possibleVerb == null) ? new ArgumentException("传入的命令行参数中没有发现谓词，但此方法规定的所有命令行类型都要求有谓词。", "possibleVerb") : new ArgumentException("没有任何命令行参数类型符合谓词 " + possibleVerb + "。", "possibleVerb");
		}
	}
}
