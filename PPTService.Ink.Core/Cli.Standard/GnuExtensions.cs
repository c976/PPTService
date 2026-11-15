using System;
using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli.Standard
{
	
	
	public static class GnuExtensions
	{
		public static CommandLineHandlerBuilder AddStandardHandlers(this ICommandLineHandlerBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			return builder.AddFilter(new GnuFilter(builder.CommandLine));
		}

		public static CommandLineAsyncHandlerBuilder AddStandardHandlers(this ICommandLineAsyncHandlerBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}
			return builder.AddFilter(new GnuFilter(builder.CommandLine));
		}
	}
}
