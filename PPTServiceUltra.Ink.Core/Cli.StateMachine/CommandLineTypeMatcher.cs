using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTServiceUltra.Ink.Core.Cli.StateMachine
{
	[StructLayout(LayoutKind.Auto)]
	
	
	
	[DebuggerDisplay("CommandLineTypeMatcher->{VerbType}")]
	internal struct CommandLineTypeMatcher<T>
	{
		public Type VerbType { get; }

		
		
		public Func<string, CommandLineTypeMatchResult<T>> Match
		{
			
			get;
		}

		public CommandLineTypeMatcher(Type verbType,  Func<string, CommandLineTypeMatchResult<T>> handler)
		{
			if ((object)verbType == null)
			{
				throw new ArgumentNullException("verbType");
			}
			VerbType = verbType;
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			Match = handler;
		}
	}
}
