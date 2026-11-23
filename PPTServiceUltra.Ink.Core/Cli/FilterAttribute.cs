using System;

namespace PPTServiceUltra.Ink.Core.Cli
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class FilterAttribute : CommandLineAttribute
	{
	}
}
