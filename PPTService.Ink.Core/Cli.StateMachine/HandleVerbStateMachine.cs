using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli.StateMachine
{
	internal class HandleVerbStateMachine< T>
	{
		
		private readonly IEnumerable<CommandLineTypeMatcher<T>> _verbMatches;

		public HandleVerbStateMachine( params CommandLineTypeMatcher<T>[] handlers)
		{
			_verbMatches = handlers;
		}

		public HandleVerbStateMachine( IEnumerable<CommandLineTypeMatcher<T>> handlers)
		{
			_verbMatches = handlers;
		}

		
		
		public IEnumerable<CommandLineTypeMatchResult<T>> Find(string verb)
		{
			CommandLineTypeMatchResult<T> commandLineTypeMatchResult = default(CommandLineTypeMatchResult<T>);
			foreach (CommandLineTypeMatcher<T> verbMatch in _verbMatches)
			{
				CommandLineTypeMatchResult<T> commandLineTypeMatchResult2 = verbMatch.Match(verb);
				switch (commandLineTypeMatchResult2.MachingResult)
				{
				case VerbMatchingResult.FallbackMatched:
					commandLineTypeMatchResult = commandLineTypeMatchResult2;
					break;
				case VerbMatchingResult.Matched:
					yield return commandLineTypeMatchResult2;
					yield break;
				}
			}
			if (commandLineTypeMatchResult.MachingResult == VerbMatchingResult.FallbackMatched)
			{
				yield return commandLineTypeMatchResult;
			}
		}
	}
}
