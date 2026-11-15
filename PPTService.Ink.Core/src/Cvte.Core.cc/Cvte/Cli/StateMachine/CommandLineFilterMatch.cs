using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PPTService.Ink.Core.Cli.StateMachine
{
	[StructLayout(LayoutKind.Auto)]
	
	
	
	[DebuggerDisplay("CommandLineFilterMatch->{FilterType}")]
	internal struct CommandLineFilterMatch
	{
		public Type FilterType { get; }

		public Func<ICommandLineFilter> FilterCreator { get; }

		public CommandLineFilterMatch(Type verbType, Func<ICommandLineFilter> filterCreator)
		{
			if ((object)verbType == null)
			{
				throw new ArgumentNullException("verbType");
			}
			FilterType = verbType;
			if (filterCreator == null)
			{
				throw new ArgumentNullException("filterCreator");
			}
			FilterCreator = filterCreator;
		}
	}
}
