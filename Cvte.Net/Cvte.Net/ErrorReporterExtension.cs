using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Cvte.Net.WebServices.Myou;

namespace Cvte.Net
{
	public static class ErrorReporterExtension
	{
		
		public static async Task ReportErrorAsync(this IErrorReporter reporter, UnhandledExceptionEventArgs args)
		{
			Exception exception = (args.ExceptionObject as Exception) ?? new Exception(args.ExceptionObject.ToString());
			await reporter.ReportErrorAsync(exception, string.Empty, ErrorPriority.Normal, string.Empty);
		}
	}
}
