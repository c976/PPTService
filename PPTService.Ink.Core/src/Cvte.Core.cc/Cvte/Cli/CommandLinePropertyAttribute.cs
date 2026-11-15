using System.Runtime.CompilerServices;

namespace PPTService.Ink.Core.Cli
{
	
	
	public abstract class CommandLinePropertyAttribute : CommandLineAttribute
	{
		internal string TypeName { get; set; }

		internal CommandLinePropertyAttribute()
		{
		}
	}
}
