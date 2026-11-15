using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Cli.StateMachine
{
	[StructLayout(LayoutKind.Auto)]
	
	
	
	internal struct CommandLineTypeMatchResult<T>
	{
		public readonly VerbMatchingResult MachingResult;

		
		public readonly Func<T> Handler;

		public Type VerbType { get; }

		
		
		public string Verb
		{
			
			get;
		}

		public CommandLineTypeMatchResult(VerbMatchingResult matchingResult, Type verbType,  string verb)
		{
			if (matchingResult != VerbMatchingResult.NotMatch)
			{
				throw new ArgumentException("当使用此重载时，VerbMatchingResult.NotMatched 是唯一有效的参数值。", "matchingResult");
			}
			MachingResult = matchingResult;
			if ((object)verbType == null)
			{
				throw new ArgumentNullException("verbType");
			}
			VerbType = verbType;
			Verb = verb;
			Handler = null;
		}

		public CommandLineTypeMatchResult(VerbMatchingResult matchingResult, Type verbType,  string verb, Func<T> handler)
		{
			if (matchingResult != VerbMatchingResult.Matched && matchingResult != VerbMatchingResult.FallbackMatched)
			{
				throw new ArgumentException("当使用此重载时，只有 VerbMatched 和 NonVerbMatched 是有效的参数值。", "matchingResult");
			}
			MachingResult = matchingResult;
			if ((object)verbType == null)
			{
				throw new ArgumentNullException("verbType");
			}
			VerbType = verbType;
			Verb = verb;
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			Handler = handler;
		}
	}
}
