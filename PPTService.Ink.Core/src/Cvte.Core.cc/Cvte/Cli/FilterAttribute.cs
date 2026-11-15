using System;

namespace PPTService.Ink.Core.Cli
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class FilterAttribute : CommandLineAttribute
	{
	}
}
